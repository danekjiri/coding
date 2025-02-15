﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTCWallet.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NBitcoin;

namespace BTCWallet.ViewModels
{
    /// <summary>
    /// Provides the base functionality for view models in the BTC Wallet application.
    /// </summary>
    public class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// The Bitcoin network used for this wallet (TestNet).
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected static readonly Network BTCNetwork = Network.TestNet;

        /// <summary>
        /// The default number of auto-generated addresses.
        /// </summary>
        protected static readonly long AutoGeneratedAddressNumber = 10;

        /// <summary>
        /// The default URI for the RPC server.
        /// </summary>
        protected const string DefaultRpcUri = "https://nd-654-980-953.p2pify.com";

        /// <summary>
        /// The default username for the RPC server.
        /// </summary>
        protected const string DefaultRpcUsername = "musing-kepler";

        /// <summary>
        /// The default password for the RPC server.
        /// </summary>
        protected const string DefaultRpcPassword = "rename-impose-shawl-gnarly-haiku-scone";

        /// <summary>
        /// Determines if the wallet is read-only.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating if the wallet is read-only.</returns>
        protected async Task<bool> IsWalletReadOnlyAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var isRO = await (from prof in db.Profiles
                              where prof.WalletId == walletId
                              select prof!.Wallet!.IsReadOnly).FirstAsync();

            return isRO == 1;
        }

        /// <summary>
        /// Retrieves the last unused receiving address.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="recAddressDerivationIndex">The derivation index for the receiving address.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the last unused receiving address.</returns>
        protected async Task<string> GetLastUnusedRecAddressAsync(long walletId, int recAddressDerivationIndex)
        {
            await using var db = new WalletDbContext();
            var addresses = await (from addr in db.Addresses
                                   where addr!.PublicKey!.WalletId == walletId
                                         && addr!.IsChange == 0
                                   select addr.AddressWif).ToListAsync();

            return addresses[recAddressDerivationIndex];
        }

        /// <summary>
        /// Retrieves the index of the last unused receiving derivation address.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the index of the last unused receiving derivation address.</returns>
        protected async Task<uint> GetLastUnusedRecDerivationAddressIndexAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var index = from prof in db.Profiles
                        where prof.WalletId == walletId
                        select prof.LastUnusedAddrIndexRec;

            return (uint)await index.FirstAsync();
        }

        /// <summary>
        /// Retrieves the index of the last unused change derivation address.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the index of the last unused change derivation address.</returns>
        protected async Task<uint> GetLastUnusedChanDerivationAddressIndexAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var index = from prof in db.Profiles
                        where prof.WalletId == walletId
                        select prof.LastUnusedAddrIndexChan;

            return (uint)await index.FirstAsync();
        }

        /// <summary>
        /// Saves a new read-only HD public key and address.
        /// </summary>
        /// <param name="derivedMasterPubKey">The derived master public key.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="walletChain">The wallet chain.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task SaveNewROPubKeyAndAddressAsync(ExtPubKey derivedMasterPubKey, uint sequenceNumber, long walletId, uint walletChain)
        {
            await using var db = new WalletDbContext();
            var key = derivedMasterPubKey.Derive(sequenceNumber);

            var publicKey = new PublicKey()
            {
                PublicKeyWif = key.GetPublicKey().ToString(),
                ChainCode = Convert.ToHexString(key.ChainCode),
                SequenceNumber = sequenceNumber,
                IsCompressed = 1,
                WalletId = walletId
            };
            await db.AddAsync(publicKey);
            await db.SaveChangesAsync();

            var address = new Address()
            {
                AddressWif = key.GetPublicKey().GetAddress(ScriptPubKeyType.Legacy, BTCNetwork).ToString(),
                PublicKeyId = publicKey.PublicKeyId,
                IsChange = walletChain,
                Network = "testnet"
            };
            await db.AddAsync(address);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Saves a new default HD public key and address.
        /// </summary>
        /// <param name="derivedMasterKey">The derived master key.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="walletChain">The wallet chain.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task SaveDefaultNewPubKeyAndAddressAsync(ExtKey derivedMasterKey, uint sequenceNumber, long walletId, uint walletChain)
        {
            await using var db = new WalletDbContext();
            var key = derivedMasterKey.Derive(sequenceNumber);
            var publicKey = new PublicKey()
            {
                PublicKeyWif = key.GetPublicKey().ToString(),
                PrivateKeyWif = key.PrivateKey.GetWif(BTCNetwork).ToString(),
                ChainCode = Convert.ToHexString(key.ChainCode),
                SequenceNumber = sequenceNumber,
                IsCompressed = 1,
                WalletId = walletId
            };
            await db.AddAsync(publicKey);
            await db.SaveChangesAsync();

            var address = new Address()
            {
                AddressWif = key.GetPublicKey().GetAddress(ScriptPubKeyType.Legacy, BTCNetwork).ToString(),
                PublicKeyId = publicKey.PublicKeyId,
                IsChange = walletChain,
                Network = "testnet"
            };
            await db.AddAsync(address);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Generates a new HD address.
        /// </summary>
        /// <param name="walletChain">The wallet chain.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="seqNumber">The sequence number.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        // ReSharper disable once InconsistentNaming
        protected async Task GenerateNewHDAddressAsync(uint walletChain, long walletId, uint seqNumber)
        {
            Wallet wallet;
            {
                await using var db = new WalletDbContext();
                wallet = await (from wall in db.Wallets
                                where wall.WalletId == walletId
                                select wall).FirstAsync();
            }
            var isRO = await IsWalletReadOnlyAsync(walletId);

            if (isRO)
            {
                var extPubKey = ExtPubKey.Parse(wallet.XpubWif, BTCNetwork)
                    .Derive(walletChain);
                await SaveNewROPubKeyAndAddressAsync(extPubKey, seqNumber, walletId, walletChain);
            }
            else
            {
                var extKey = ExtKey.Parse(wallet.MasterKeyWif!, BTCNetwork)
                    .Derive(new KeyPath(wallet.Derivation!)).Derive(walletChain);
                await SaveDefaultNewPubKeyAndAddressAsync(extKey, seqNumber, walletId, walletChain);
            }
        }

        /// <summary>
        /// Gets a list of addresses.
        /// </summary>
        /// <param name="areChangeAddresses">A boolean indicating whether to get change addresses.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of addresses.</returns>
        protected async Task<List<Address>> GetAddressesListAsync(bool areChangeAddresses, long walletId)
        {
            await using var db = new WalletDbContext();

            return await (from address in db.Addresses
                          where address!.PublicKey!.WalletId == walletId
                                && (areChangeAddresses ? 1 : 0) == address.IsChange
                          select address).ToListAsync();
        }

        /// <summary>
        /// Gets a list of public keys.
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of public keys.</returns>
        protected async Task<List<PublicKey>> GetPublicKeysAsync(long walletId)
        {
            await using var db = new WalletDbContext();
            var pubKeysList = await (from address in db.Addresses
                                     where address!.PublicKey!.WalletId == walletId
                                     select address.PublicKey).ToListAsync();

            return pubKeysList;
        }

        /// <summary>
        /// Gets the script for an address.
        /// </summary>
        /// <param name="seqNumber">The sequence number.</param>
        /// <param name="walletId">The wallet ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the script for the address.</returns>
        protected async Task<Script> GetAddressScriptAsync(uint seqNumber, long walletId)
        {
            await using var db = new WalletDbContext();
            var currentWallet = await db.FindAsync<Wallet>(walletId);
            return ExtPubKey.Parse(currentWallet!.XpubWif, BTCNetwork)
                .Derive(0).Derive(seqNumber).ScriptPubKey;
        }
    }
}