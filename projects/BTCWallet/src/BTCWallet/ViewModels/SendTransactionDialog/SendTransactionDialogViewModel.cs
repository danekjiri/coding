using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using NBitcoin.RPC;
using Transaction = NBitcoin.Transaction;

namespace BTCWallet.ViewModels.SendTransactionDialog
{
    /// <summary>
    /// ViewModel for handling the logic for sending a Bitcoin transaction.
    /// </summary>
    public partial class SendTransactionDialogViewModel : ViewModelBase
    {
        private readonly long _walletId;
        private Transaction? _transaction;
        private readonly RPCClient? _rpcClient;

        /// <summary>
        /// Collection of input UTXOs used for the transaction.
        /// </summary>
        public Collection<Output> InputUTXOs { get; private set; } = new();

        /// <summary>
        /// Collection of output UTXOs created by the transaction.
        /// </summary>
        public Collection<Output> OutputUTXOs { get; private set; } = new();

        /// <summary>
        /// Gets the hexadecimal representation of the transaction.
        /// </summary>
        public string? TransactionHex => _transaction?.ToHex();

        /// <summary>
        /// Gets the hash of the transaction.
        /// </summary>
        public uint256? TransactionHash => _transaction?.GetHash();

        /// <summary>
        /// Gets the size of the transaction in bytes.
        /// </summary>
        public int TransactionSize => _transaction.GetSerializedSize();

        /// <summary>
        /// Gets the total value of the transaction outputs in satoshi.
        /// </summary>
        public long? TransactionTotalValue => _transaction?.TotalOut.Satoshi;

        [ObservableProperty] private string _loadingError = "Unknown Error.";
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private bool _isBroadcasted;
        [ObservableProperty] private BitcoinAddress? _recipientAddress;
        [ObservableProperty] private BitcoinAddress? _changeAddress;
        [ObservableProperty] private long _fee;
        [ObservableProperty] private long _amount;
        [ObservableProperty] private long _changeAmount;
        [ObservableProperty] private string? _comment;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendTransactionDialogViewModel"/> class.
        /// </summary>
        /// <param name="walletId">The ID of the wallet.</param>
        /// <param name="rpcClient">The RPC client to interact with the Bitcoin network.</param>
        /// <param name="address">The recipient address.</param>
        /// <param name="amount">The amount to send in satoshi.</param>
        /// <param name="fee">The transaction fee in satoshi.</param>
        /// <param name="changeAmount">The change amount in satoshi.</param>
        /// <param name="comment">The transaction comment.</param>
        /// <param name="spendingUTXOs">The UTXOs to spend in the transaction.</param>
        public SendTransactionDialogViewModel(long walletId, RPCClient rpcClient, string address, long amount, long fee,
            long changeAmount, string? comment, Collection<Output> spendingUTXOs)
        {
            _walletId = walletId;
            Fee = fee;
            Amount = amount;
            ChangeAmount = changeAmount;
            Comment = comment;
            _rpcClient = rpcClient;

            Initialize(address, spendingUTXOs);
        }

        /// <summary>
        /// Gets the change address for the wallet asynchronously.
        /// </summary>
        /// <returns>The change address as a <see cref="BitcoinPubKeyAddress"/>.</returns>
        private async Task<BitcoinPubKeyAddress> GetChangeAddressAsync()
        {
            await using var db = new WalletDbContext();
            var changeAddrIndex = await
                (from profile in db.Profiles
                 where profile.WalletId == _walletId
                 select profile.LastUnusedAddrIndexChan).FirstOrDefaultAsync();
            var changeAddress = await
                (from addr in db.Addresses
                 where addr.PublicKey!.WalletId == _walletId
                       && addr.IsChange == 1
                       && addr.PublicKey.SequenceNumber == changeAddrIndex
                 select addr).FirstOrDefaultAsync();

            return new BitcoinPubKeyAddress(changeAddress!.AddressWif, BTCNetwork);
        }

        /// <summary>
        /// Creates a partial output from an address.
        /// </summary>
        /// <param name="output">The transaction output.</param>
        /// <param name="outputIndex">The output index.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the partial output.</returns>
        private async Task<Output> CreatePartialOutputFromAddress(TxOut output, long outputIndex)
        {
            await using var db = new WalletDbContext();
            var foundAddressDb = await
                (from addr in db.Addresses
                 where addr.AddressWif == output!.ScriptPubKey!.GetDestinationAddress(BTCNetwork)!.ToString()
                       && addr!.PublicKey!.WalletId == _walletId
                 select addr).FirstOrDefaultAsync();
            if (foundAddressDb is null)
            {
                return new Output()
                {
                    AddressId = null,
                    AmountSatoshi = output.Value.Satoshi,
                    AddressWif = output.ScriptPubKey.GetDestinationAddress(BTCNetwork)!.ToString(),
                    ScriptPubKey = output.ScriptPubKey.ToHex(),
                    IsSpent = 0,
                    OutputIndex = outputIndex
                };
            }

            return new Output()
            {
                AddressId = foundAddressDb.AddressId,
                AmountSatoshi = output.Value.Satoshi,
                AddressWif = output.ScriptPubKey.GetDestinationAddress(BTCNetwork)!.ToString(),
                ScriptPubKey = output.ScriptPubKey.ToHex(),
                IsSpent = 0,
                OutputIndex = outputIndex
            };
        }

        /// <summary>
        /// Creates a signed transaction hex asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the signed transaction.</returns>
        private async Task<Transaction> CreateSignedTransactionHexAsync()
        {
            await using var db = new WalletDbContext();
            var sendingUTXOPrivateKeys = new Collection<BitcoinSecret>();
            var sendingCoins = new List<Coin>();

            foreach (var input in InputUTXOs)
            {
                var tx = await db.FindAsync<DataModels.Transaction>(input.TransactionId);
                var pubkey = await db.FindAsync<PublicKey>(input.AddressId);

                var prvkey = new BitcoinSecret(pubkey!.PrivateKeyWif, BTCNetwork);
                sendingUTXOPrivateKeys.Add(prvkey);

                sendingCoins.Add(
                    new Coin(
                        uint256.Parse(tx!.TxHash),
                        (uint)input.OutputIndex, input.AmountSatoshi,
                        prvkey.PrivateKey.GetScriptPubKey(ScriptPubKeyType.Legacy))
                    );
            }

            var transaction = Transaction.Create(BTCNetwork);
            foreach (var input in sendingCoins)
            {
                transaction.Inputs.Add(input.Outpoint, input.ScriptPubKey);
            }

            transaction.Outputs.Add(new Money(Amount, MoneyUnit.Satoshi), RecipientAddress!.ScriptPubKey);
            if (ChangeAmount > 0)
                transaction.Outputs.Add(new Money(ChangeAmount, MoneyUnit.Satoshi), ChangeAddress!.ScriptPubKey);

            transaction.Sign(sendingUTXOPrivateKeys, sendingCoins);

            return transaction;
        }

        /// <summary>
        /// Initializes the view model by setting recipient address, change address, and creating the transaction.
        /// </summary>
        /// <param name="address">The recipient address.</param>
        /// <param name="spendingUTXOs">The UTXOs to spend in the transaction.</param>
        private async void Initialize(string address, Collection<Output> spendingUTXOs)
        {
            RecipientAddress = new BitcoinPubKeyAddress(address, BTCNetwork);
            ChangeAddress = await GetChangeAddressAsync();

            InputUTXOs = spendingUTXOs;

            _transaction = await CreateSignedTransactionHexAsync();

            OutputUTXOs = new Collection<Output>();
            for (int i = 0; i < _transaction.Outputs.Count; i++)
            {
                OutputUTXOs.Add(await CreatePartialOutputFromAddress(_transaction.Outputs[i], i));
            }
        }

        /// <summary>
        /// Cancels the transaction by closing the broadcast window.
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            CloseBroadcastWindow();
        }

        /// <summary>
        /// Broadcasts the transaction to the Bitcoin network.
        /// </summary>
        [RelayCommand]
        private async Task BroadcastTx()
        {
            IsError = false;
            try
            {
                await using var db = new WalletDbContext();
                var txhash = await _rpcClient!.SendRawTransactionAsync(_transaction);
                if (txhash is null)
                    return;

                foreach (var input in InputUTXOs)
                {
                    (await db.FindAsync<Output>(input.OutputId))!.IsSpent = 1;
                }

                var profile = await db.FindAsync<Profile>(_walletId);
                var changeAddressDerivationIndex = await GetLastUnusedChanDerivationAddressIndexAsync(_walletId);
                profile!.LastUnusedAddrIndexChan = ++changeAddressDerivationIndex;
                await GenerateNewHDAddressAsync
                    (walletChain: 1, _walletId, (uint)(changeAddressDerivationIndex + AutoGeneratedAddressNumber));

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                IsError = true;
                if (ex is RPCException)
                {
                    LoadingError = "Invalid UTXOs, refresh and try again.";
                }
                else
                    throw;
            }
            finally
            {
                if (!IsError)
                    IsBroadcasted = true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                CloseBroadcastWindow();
            }
        }

        /// <summary>
        /// Closes the broadcast window.
        /// </summary>
        private void CloseBroadcastWindow()
        {
            var profileDialogWindow =
                from windows in MainWindowViewModel.GetAllWindows()
                where windows.Name == "BroadcastDialog"
                select windows;
            profileDialogWindow.First().Close();
        }
    }
}
