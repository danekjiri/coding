using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class PublicKey
    {
        public PublicKey()
        {
            Addresses = new HashSet<Address>();
        }

        public long PublicKeyId { get; set; }
        public string PublicKeyWif { get; set; } = null!;
        public string? PrivateKeyWif { get; set; }
        public string ChainCode { get; set; } = null!;
        public long IsCompressed { get; set; }
        public long SequenceNumber { get; set; }
        public long? WalletId { get; set; }

        public virtual Wallet? Wallet { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
