using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using BTCWallet.DataModels;
using BTCWallet.Models;
using BTCWallet.ViewModels.SendTransactionDialog;
using BTCWallet.Views.SendTransactionDialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;

namespace BTCWallet.ViewModels.WalletProfile
{
    /// <summary>
    /// ViewModel for sending transactions in the Bitcoin wallet application.
    /// </summary>
    public partial class WalletProfileSendViewModel : ViewModelBase
    {
        private readonly long _walletId;
    
        [ObservableProperty] private bool _rpcConfigOn;
        [ObservableProperty] private string _rpcUri;
        [ObservableProperty] private string _rpcUser;
        [ObservableProperty] private string _rpcPassword;

        [ObservableProperty] private string? _payToAddress;
        [ObservableProperty] private string? _comment;
        [ObservableProperty] private string? _amount;
        [ObservableProperty] private string? _fee;

        partial void OnPayToAddressChanged(string? value)
        {
            SendTransactionCommand.NotifyCanExecuteChanged();
        }
        
        partial void OnFeeChanged(string? value)
        {
            SendTransactionCommand.NotifyCanExecuteChanged();
        }
        
        partial void OnAmountChanged(string? value)
        {
            SendTransactionCommand.NotifyCanExecuteChanged();
        }

        // ReSharper disable once InconsistentNaming
        private long _amountLong { get; set; }
        // ReSharper disable once InconsistentNaming
        private long _feeLong { get; set; }
        // ReSharper disable once InconsistentNaming
        private long _spendingUTXOsSum { get; set; }

        // ReSharper disable once InconsistentNaming
        [ObservableProperty] private string? _feeHint30m = "not loaded.";
        // ReSharper disable once InconsistentNaming
        [ObservableProperty] private string? _feeHint60m = "not loaded.";
        // ReSharper disable once InconsistentNaming
        [ObservableProperty] private string? _feeHint24h = "not loaded.";

        [ObservableProperty] private bool _minimalizeUTXOSpending;
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private string _loadingErrorRPC = "Unknown error, try again.";

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletProfileSendViewModel"/> class.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        public WalletProfileSendViewModel(long walletId)
        {
            _walletId = walletId;
            RpcUri = DefaultRpcUri;
            RpcUser = DefaultRpcUsername;
            RpcPassword = DefaultRpcPassword;
            Initialize();
        }

        private async void Initialize()
        { 
            await GetFeeRateEstimationsAsync();
        }

        /// <summary>
        /// Gets fee rate estimations from an external API.
        /// </summary>
        private async Task GetFeeRateEstimationsAsync()
        {
            const string restapiUrl = "https://bitcoiner.live/api/fees/estimates/latest";

            using var httpClient = new HttpClient();
            try
            {
                var jsonString = await httpClient.GetStringAsync(restapiUrl);
                Root? data = JsonConvert.DeserializeObject<Root>(jsonString);

                if (data is null)
                    throw new FormatException();
                FeeHint30m = data.Estimates?.Thirty?.Total?.P2PKH?.Satoshi.ToString(CultureInfo.CurrentCulture);
                FeeHint60m = data.Estimates?.Sixty?.Total?.P2PKH?.Satoshi.ToString(CultureInfo.CurrentCulture);
                FeeHint24h = data.Estimates?.FourteenForty?.Total?.P2PKH?.Satoshi.ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException or FormatException or HttpRequestException)
                {
                    IsError = true;
                    LoadingErrorRPC = "Bad internet connection, try again.";
                }
                else
                    throw;
            }
        }

        /// <summary>
        /// Checks if the RPC configuration is valid.
        /// </summary>
        /// <returns>True if the RPC configuration is valid; otherwise, false.</returns>
        private bool IsRpcConfigValid()
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
        /// Checks if the transaction can be sent.
        /// </summary>
        /// <returns>True if the transaction can be sent; otherwise, false.</returns>
        private bool CanSendTransaction()
        {
            IsError = false;
            if (string.IsNullOrEmpty(PayToAddress))
            {
                IsError = true; LoadingErrorRPC = "Fill address!" ; return false;
            }
            if (string.IsNullOrEmpty(Amount))
            {
                IsError = true; LoadingErrorRPC = "Fill amount!" ; return false;
            }
            if (string.IsNullOrEmpty(Fee))
            {
                IsError = true; LoadingErrorRPC = "Fill fee!" ; return false;
            }

            if (!IsRpcConfigValid())
            {
                IsError = true; LoadingErrorRPC = "Invalid credential!" ; return false;
            }
            
            try
            {
                _ = BitcoinAddress.Create(PayToAddress, BTCNetwork);
                _amountLong = long.Parse(Amount);
                _feeLong = long.Parse(Fee);
            }
            catch (Exception ex)
            {
                if (ex is FormatException or OverflowException)
                {
                    IsError = true;
                    LoadingErrorRPC = "Not valid Amount/Fee";
                    return false;
                }
                throw;
            }
            
            if (_feeLong < 400)
            {
                IsError = true;
                LoadingErrorRPC = "The fee is too low, tx will not be accepted.";
                return false;
            }
        
            using var db = new WalletDbContext();
            var totalAmountSats =
                (from addr in db.Addresses
                join output in db.Outputs on addr.AddressId equals output.AddressId
                where addr!.PublicKey!.WalletId == _walletId && output!.IsSpent == 0
                select output.AmountSatoshi).Sum();
            if (totalAmountSats < _amountLong + _feeLong)
            {
                IsError = true;
                LoadingErrorRPC = "Not enough satoshis!";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the list of UTXOs for the wallet.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<List<Output>> GetWalletsUTXOsList()
        {
            await using var db = new WalletDbContext();
            var utxos = await
                (from addr in db.Addresses
                    join output in db.Outputs on addr.AddressId equals output.AddressId
                    where addr!.PublicKey!.WalletId == _walletId && output!.IsSpent == 0
                    select output).ToListAsync();
            return utxos;
        }

        /// <summary>
        /// Gets the minimalized UTXOs for spending.
        /// </summary>
        /// <param name="utxos">The list of UTXOs.</param>
        /// <returns>A collection of UTXOs for spending.</returns>
        private Collection<Output> GetMinimalizedUTXOsSpending(List<Output> utxos)
        {
            var sortedUTXOs = utxos.OrderByDescending(o => o.AmountSatoshi);
            Collection<Output> spendingUTXOs = new();
            _spendingUTXOsSum = 0;

            foreach (var utxo in sortedUTXOs)
            {
                if (_spendingUTXOsSum < _amountLong + _feeLong)
                {
                    spendingUTXOs.Add(utxo);
                    _spendingUTXOsSum += utxo.AmountSatoshi;
                }
                else
                    break;
            }
        
            _spendingUTXOsSum -= _amountLong + _feeLong;
            return spendingUTXOs;
        }

        /// <summary>
        /// Gets the maximalized UTXOs for spending.
        /// </summary>
        /// <param name="utxos">The list of UTXOs.</param>
        /// <returns>A collection of UTXOs for spending.</returns>
        private Collection<Output> GetMaximalizedUTXOsSpending(List<Output> utxos)
        {
            var sortedUTXOs = utxos.OrderBy(o => o.AmountSatoshi);
            Collection<Output> spendingUTXOs = new();
            _spendingUTXOsSum = 0;

            foreach (var utxo in sortedUTXOs)
            {
                if (_spendingUTXOsSum < _amountLong + _feeLong)
                {
                    spendingUTXOs.Add(utxo);
                    _spendingUTXOsSum += utxo.AmountSatoshi;
                }
                else
                    break;
            }

            _spendingUTXOsSum -= _amountLong + _feeLong;
            return spendingUTXOs;
        }

        /// <summary>
        /// Opens the broadcast transaction window.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="rpcClient">The RPC client.</param>
        /// <param name="address">The address to send to.</param>
        /// <param name="amount">The amount to send.</param>
        /// <param name="fee">The fee for the transaction.</param>
        /// <param name="spendingUTXOsSum">The sum of the spending UTXOs.</param>
        /// <param name="comment">The comment for the transaction.</param>
        /// <param name="spendingUTXOs">The collection of spending UTXOs.</param>
        private void OpenBroadcastTransactionWindow(long walletId, RPCClient rpcClient, string address, long amount, long fee,
            long spendingUTXOsSum, string? comment, Collection<Output> spendingUTXOs)
        {
            var win = new Window()
            {
                Height = 550,
                Width = 800,
                MinHeight = 520,
                MinWidth = 800,
                Content = ((Control)Activator.CreateInstance(typeof(SendTransactionDialogView))!)
                    .DataContext = 
                    new SendTransactionDialogViewModel(walletId, rpcClient, address, amount, fee, spendingUTXOsSum,
                        comment, spendingUTXOs),
                Name = "BroadcastDialog",
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome,
                ExtendClientAreaToDecorationsHint = true
            };
            win.Show();
        }

        /// <summary>
        /// Sends the transaction.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand(CanExecute = nameof(CanSendTransaction))]
        private async Task SendTransaction()
        {
            try
            {
                Collection<Output> spendingUTXOs;
                var credentials = new NetworkCredential(RpcUser, RpcPassword);
                var rpcClient = new RPCClient(credentials, new Uri(RpcUri), Network.TestNet);
                await rpcClient.UptimeAsync();

                if (MinimalizeUTXOSpending)
                    spendingUTXOs = GetMinimalizedUTXOsSpending(await GetWalletsUTXOsList());
                else
                    spendingUTXOs = GetMaximalizedUTXOsSpending(await GetWalletsUTXOsList());

                OpenBroadcastTransactionWindow(_walletId, rpcClient, PayToAddress!, _amountLong, _feeLong,
                    _spendingUTXOsSum, Comment, spendingUTXOs);

                PayToAddress = Comment = Amount = Fee = string.Empty;
            }
            catch (Exception ex)
            {
                IsError = true;
                if (ex is RPCException)
                {
                    LoadingErrorRPC = "Unknown Error, try again...";
                }
                else if (ex is HttpRequestException)
                {
                    if (ex.Message.Contains("nodename nor servname provided, or not known"))
                    {
                        LoadingErrorRPC = "Uri or Connection Error";
                    }
                    else if (ex.Message.Contains("Response status code does not indicate success"))
                    {
                        LoadingErrorRPC = "Credentials Error";
                    }
                }
            }
        }
    }
}
