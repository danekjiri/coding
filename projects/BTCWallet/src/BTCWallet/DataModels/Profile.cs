using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Profile
    {
        public long ProfileId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public long LastUnusedAddrIndexRec { get; set; }
        public long LastUnusedAddrIndexChan { get; set; }
        public long? WalletId { get; set; }

        public virtual Wallet? Wallet { get; set; }
    }
}
