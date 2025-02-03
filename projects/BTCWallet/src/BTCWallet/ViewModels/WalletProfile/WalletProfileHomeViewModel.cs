using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using BTCWallet.DataModels;
using BTCWallet.Models;
using BTCWallet.Views.WalletProfile;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using NBitcoin.RPC;
using NBitcoin.Scripting;
using Transaction = NBitcoin.Transaction;

namespace BTCWallet.ViewModels.WalletProfile
{
    /// <summary>
    /// ViewModel for the Wallet Profile Home view.
    /// </summary>
    public partial class WalletProfileHomeViewModel : ViewModelBase
    {
        private readonly long _walletId;
        private RPCClient? _rpcClient;

        [ObservableProperty] private bool _rpcConfigOn;
        partial void OnRpcConfigOnChanged(bool value)
        {
            if (!value)
            {
                _rpcClient = null;
                RpcUri = DefaultRpcUri;
                RpcUser = DefaultRpcUsername;
                RpcPassword = DefaultRpcPassword;
            }
        }

        [ObservableProperty] private bool _isRefreshEnabled = true;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private bool _isBadConnection = true;

        [ObservableProperty] private long _amountSum;
        [ObservableProperty] private double _usdBtcRate;
        [ObservableProperty] private double _usdValue;
        [ObservableProperty] private long _txCount;

        [ObservableProperty] private string _loadingErrorRPC = "Unknown Error, try again.";
        [ObservableProperty] private string _rpcUri;

        partial void OnRpcUriChanged(string value)
        {
            IsRefreshEnabled = true;
        }

        [ObservableProperty] private string _rpcUser;

        partial void OnRpcUserChanged(string value)
        {
            IsRefreshEnabled = true;
        }

        [ObservableProperty] private string _rpcPassword;

        partial void OnRpcPasswordChanged(string value)
        {
            IsRefreshEnabled = true;
        }

        [ObservableProperty] private ObservableCollection<HomeUTXOReview>? _UTXOs;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletProfileHomeViewModel"/> class.
        /// </summary>
        /// <param name="walletId">The wallet ID associated with this view model.</param>
        public WalletProfileHomeViewModel(long walletId)
        {
            _walletId = walletId;
            RpcUri = DefaultRpcUri;
            RpcUser = DefaultRpcUsername;
            RpcPassword = DefaultRpcPassword;

            Initialize();
        }

        /// <summary>
        /// Initializes the view model by loading UTXOs and updating the amount and transaction count.
        /// </summary>
        private async void Initialize()
        {
            UTXOs = new ObservableCollection<HomeUTXOReview>(await GetListOfAllUTXOsFromDatabaseAsync());
            AmountSum = (from tx in UTXOs select tx.Amount).Sum();
            TxCount = UTXOs.Count;

            UsdBtcRate = await GetExchangeRateBtcToUsdAsync();
            UsdValue = ConvertSatoshiToUsd(AmountSum, UsdBtcRate);
        }

        /// <summary>
        /// Retrieves the current exchange rate from BTC to USD.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the BTC to USD exchange rate.</returns>
        private async Task<double> GetExchangeRateBtcToUsdAsync()
        {
            IsBadConnection = false;
            const string restapiUrl = "https://api.coingate.com/v2/rates/merchant/BTC/USD";

            using var httpClient = new HttpClient();
            try
            {
                var value = await httpClient.GetStringAsync(restapiUrl);
                return (double.Parse(value, CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException or FormatException or HttpRequestException)
                {
                    IsBadConnection = true;
                    return 0;
                }

                throw;
            }
        }

        /// <summary>
        /// Converts the given amount in Satoshis to USD.
        /// </summary>
        /// <param name="sats">The amount in Satoshis.</param>
        /// <param name="btcusdRate">The BTC to USD exchange rate.</param>
        /// <returns>The equivalent amount in USD.</returns>
        private double ConvertSatoshiToUsd(long sats, double btcusdRate)
        {
            return (double)sats / 100_000_000 * btcusdRate;
        }

        /// <summary>
        /// Retrieves a list of all unspent transaction outputs (UTXOs) from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of UTXOs.</returns>
        private async Task<List<HomeUTXOReview>> GetListOfAllUTXOsFromDatabaseAsync()
        {
            await using var db = new WalletDbContext();
            var ress =
                from output in db.Outputs
                where output.AddressId != null &&
                      output!.Address!.PublicKey!.WalletId == _walletId &&
                      output!.IsSpent == 0
                select new HomeUTXOReview()
                {
                    DateCreated = DateTime.Parse(output.Transaction.CreatedAt),
                    Address = output!.Address!.AddressWif,
                    Comment = output!.Transaction!.Comment,
                    Amount = output!.AmountSatoshi
                };

            var list = await ress.ToListAsync();
            return list;
        }

        /// <summary>
        /// Determines if the wallet can be refreshed using RPC.
        /// </summary>
        /// <returns><c>true</c> if the wallet can be refreshed using RPC; otherwise, <c>false</c>.</returns>
        private bool CanRefreshUsingRPC()
        {
            if (RpcConfigOn)
            {
                if (string.IsNullOrEmpty(RpcUser) || string.IsNullOrEmpty(RpcUri) || string.IsNullOrEmpty(RpcPassword))
                    return false;
                try
                {
                    _ = new Uri(RpcUri);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Prepares a list of addresses for scanning by the RPC client.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of output descriptors.</returns>
        private async Task<List<OutputDescriptor>> GetPreparedAddressesForScanningAsync()
        {
            var outputDescriptorList = new List<OutputDescriptor>();
            var walletAddresses = await GetPublicKeysAsync(_walletId);
            foreach (var pubkey in walletAddresses)
            {
                var outputDesc = OutputDescriptor.Parse($"pkh({pubkey.PublicKeyWif})", BTCNetwork);
                outputDescriptorList.Add(outputDesc);
            }

            return outputDescriptorList;
        }

        /// <summary>
        /// Gets the IDs of UTXOs that are unmatched between the current wallet UTXOs and newly scanned UTXOs.
        /// </summary>
        /// <param name="walletOldUTXOs">The list of old UTXOs from the wallet.</param>
        /// <param name="newUTXOs">The array of new UTXOs from the scan.</param>
        /// <returns>The list of unmatched UTXO IDs.</returns>
        private IEnumerable<long> GetUnmatchedUTXOsId(IEnumerable<Output> walletOldUTXOs, ScanTxoutOutput[] newUTXOs)
        {
            var unmatchedUTXOsId = new Collection<long>();
            foreach (var oldUTXO in walletOldUTXOs)
            {
                bool isMatched = false;
                foreach (var newUTXO in newUTXOs)
                {
                    if (oldUTXO.ScriptPubKey == newUTXO.Coin.ScriptPubKey.ToHex() &&
                        oldUTXO.AmountSatoshi == newUTXO.Coin.Amount)
                    {
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                    unmatchedUTXOsId.Add(oldUTXO.OutputId);
            }

            return unmatchedUTXOsId;
        }

        /// <summary>
        /// Marks the specified unmatched UTXOs as spent in the database.
        /// </summary>
        /// <param name="unmatchedUTXOsId">The list of unmatched UTXO IDs.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task MarkUnmatchedUTXOsAsSpentAsync(IEnumerable<long> unmatchedUTXOsId)
        {
            await using var db = new WalletDbContext();
            foreach (var id in unmatchedUTXOsId)
            {
                var output = await db.Outputs.FindAsync(id);
                output!.IsSpent = 1;
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the newly occurred UTXOs from the scan that are not present in the current wallet UTXOs.
        /// </summary>
        /// <param name="walletOldUTXOs">The list of old UTXOs from the wallet.</param>
        /// <param name="newUTXOs">The array of new UTXOs from the scan.</param>
        /// <returns>The list of newly occurred UTXOs.</returns>
        private IEnumerable<ScanTxoutOutput> GetNewlyOccuredScannedUTXOs(IEnumerable<Output> walletOldUTXOs,
            ScanTxoutOutput[] newUTXOs)
        {
            var newlyOccuredUTXOs = new Collection<ScanTxoutOutput>();
            foreach (var newUTXO in newUTXOs)
            {
                bool isNew = true;
                foreach (var oldUTXO in walletOldUTXOs)
                {
                    if (oldUTXO.ScriptPubKey == newUTXO.Coin.ScriptPubKey.ToHex() &&
                        oldUTXO.AmountSatoshi == newUTXO.Coin.Amount)
                    {
                        isNew = false;
                        break;
                    }
                }
                if (isNew)
                    newlyOccuredUTXOs.Add(newUTXO);
            }

            return newlyOccuredUTXOs;
        }

        /// <summary>
        /// Stores the incoming transaction in the database.
        /// </summary>
        /// <param name="tx">The transaction to store.</param>
        /// <param name="utxo">The UTXO associated with the transaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task StoreIncomingTransactionDbAsync(Transaction tx, ScanTxoutOutput utxo)
        {
            long totalIn = 0;
            var blockHash = await _rpcClient!.GetBlockHashAsync(utxo.Height);
            var txTime = (await _rpcClient!.GetBlockStatsAsync(blockHash)).Time.ToString("dd/MM/yyyy");
            DataModels.Transaction newTx = new()
            {
                TxHash = utxo.Coin.Outpoint.Hash.ToString(),
                CreatedAt = txTime,
                BlockHeight = utxo.Height,
                Comment = string.Empty,
                IsCreated = 0
            };

            await using var db = new WalletDbContext();
            await db.AddAsync(newTx);
            await db.SaveChangesAsync();

            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                var inputAmount = (await
                    _rpcClient!.GetRawTransactionAsync(uint256.Parse(tx.Inputs[i].PrevOut.Hash.ToString())))
                        .Outputs[tx.Inputs[i].PrevOut.N].Value.Satoshi;
                totalIn += inputAmount;
                Input newInput = new()
                {
                    TransactionId = newTx.TransactionId,
                    AmountSatoshi = inputAmount,
                    InputIndex = i,
                    PreviousOutIndex = tx.Inputs[i].PrevOut.N,
                    PreviousOutTxHash = tx.Inputs[i].PrevOut.Hash.ToString(),
                    ScriptSig = tx.Inputs[i].ScriptSig.ToHex()
                };
                await db.AddAsync(newInput);
            }

            long myAddressId = await
                (from addr in db.Addresses
                where addr.AddressWif == utxo!.Coin!.ScriptPubKey.GetDestinationAddress(BTCNetwork)!.ToString()
                    && addr!.PublicKey!.WalletId == _walletId
                select addr.AddressId).FirstAsync();
            for (int i = 0; i < tx.Outputs.Count; i++)
            {
                Output newOutput = new()
                {
                    TransactionId = newTx.TransactionId,
                    AddressId = i == utxo.Coin.Outpoint.N ? myAddressId : null,
                    AddressWif = utxo.Coin.ScriptPubKey.GetDestinationAddress(BTCNetwork)!.ToString(),
                    AmountSatoshi = utxo.Coin.Amount.Satoshi,
                    OutputIndex = utxo.Coin.Outpoint.N,
                    ScriptPubKey = utxo.Coin.ScriptPubKey.ToHex(),
                    IsSpent = 0
                };
                await db.AddAsync(newOutput);
            }

            newTx.FeeSatoshi = totalIn - tx.TotalOut.Satoshi;
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Stores transactions using the newly occurred UTXOs.
        /// </summary>
        /// <param name="newlyOccuredUTXO">The list of newly occurred UTXOs.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task StoreTransactionUsingNewlyOccuredUTXOsAsync(IEnumerable<ScanTxoutOutput> newlyOccuredUTXO)
        {
            foreach (var utxo in newlyOccuredUTXO)
            {
                var rawTx = await
                    _rpcClient!.GetRawTransactionAsync(uint256.Parse(utxo.Coin.Outpoint.Hash.ToString()));
                await StoreIncomingTransactionDbAsync(rawTx, utxo);
            }
        }

        /// <summary>
        /// Saves and updates UTXOs in the database based on the scanned UTXOs.
        /// </summary>
        /// <param name="scannedUTXOs">The scanned UTXOs.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SaveAndUpdateUTXOsDBAsync(ScanTxoutSetResponse scannedUTXOs)
        {
            IEnumerable<Output> walletOldUTXOs;
            {
                await using var db = new WalletDbContext();
                walletOldUTXOs = await
                    (from addr in db.Addresses
                    join output in db.Outputs on addr.AddressId equals output.AddressId
                    where addr!.PublicKey!.WalletId == _walletId && output!.IsSpent == 0
                    select output).ToListAsync();
            }
            var unmatchedUTXOsId = GetUnmatchedUTXOsId(walletOldUTXOs, scannedUTXOs.Outputs);
            await MarkUnmatchedUTXOsAsSpentAsync(unmatchedUTXOsId);
            var newlyOccuredUTXOs =
                GetNewlyOccuredScannedUTXOs(walletOldUTXOs, scannedUTXOs.Outputs);
            await StoreTransactionUsingNewlyOccuredUTXOsAsync(newlyOccuredUTXOs);
        }

        /// <summary>
        /// Refreshes the wallet using RPC.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand(CanExecute = nameof(CanRefreshUsingRPC))]
        private async Task RefreshUsingRPCAsync()
        {
            IsLoading = true;
            IsError = false;
            var credentials = new NetworkCredential(RpcUser, RpcPassword);
            try
            {
                _rpcClient = new RPCClient(credentials, new Uri(RpcUri), Network.TestNet);
                List<OutputDescriptor> addressesToScan =
                    await GetPreparedAddressesForScanningAsync();
                var utxos = await _rpcClient.StartScanTxoutSetAsync(new ScanTxoutSetParameters(addressesToScan));
                await SaveAndUpdateUTXOsDBAsync(utxos);
                UTXOs = new ObservableCollection<HomeUTXOReview>(await GetListOfAllUTXOsFromDatabaseAsync());
            }
            catch (Exception ex)
            {
                IsError = true;
                if (ex is ArgumentException or RPCException)
                {
                    LoadingErrorRPC = ex.Message;
                    IsRefreshEnabled = false;
                    _rpcClient = null;
                }
                else if (ex is HttpRequestException)
                {
                    LoadingErrorRPC = ex.Message;
                    IsRefreshEnabled = false;
                    _rpcClient = null;
                }
                else
                    throw;
            }
            finally
            {
                IsLoading = false;
                UsdBtcRate = await GetExchangeRateBtcToUsdAsync();
                UsdValue = ConvertSatoshiToUsd(AmountSum, UsdBtcRate);
            }
        }

        /// <summary>
        /// Opens a new export window.
        /// </summary>
        [RelayCommand]
        private void OpenNewExportWindow()
        {
            var win = new Window()
            {
                Height = 550,
                Width = 800,
                MinHeight = 520,
                MinWidth = 800,
                Content = ((Control)Activator.CreateInstance(typeof(WalletProfileExportView))!)
                    .DataContext = new WalletProfileExportViewModel(_walletId),
                Name = "ExportDialog"
            };
            win.Show();
        }
    }
}
