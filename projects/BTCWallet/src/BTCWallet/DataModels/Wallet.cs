    using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Wallet
    {
        public Wallet()
        {
            Profiles = new HashSet<Profile>();
            PublicKeys = new HashSet<PublicKey>();
        }

        public long WalletId { get; set; }
        public string? Mnemonic { get; set; }
        public string? Derivation { get; set; }
        public string? MasterKeyWif { get; set; }
        public string? Passphrase { get; set; }
        public string XpubWif { get; set; } = null!;
        public long IsReadOnly { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<PublicKey> PublicKeys { get; set; }
    }
}
