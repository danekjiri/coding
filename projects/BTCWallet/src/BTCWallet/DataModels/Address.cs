using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Address
    {
        public Address()
        {
            Outputs = new HashSet<Output>();
        }

        public long AddressId { get; set; }
        public string AddressWif { get; set; } = null!;
        public long? PublicKeyId { get; set; }
        public long IsChange { get; set; }
        public string Network { get; set; } = null!;

        public virtual PublicKey? PublicKey { get; set; }
        public virtual ICollection<Output> Outputs { get; set; }
    }
}
