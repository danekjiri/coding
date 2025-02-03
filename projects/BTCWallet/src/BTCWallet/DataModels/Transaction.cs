using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Transaction
    {
        public Transaction()
        {
            Inputs = new HashSet<Input>();
            Outputs = new HashSet<Output>();
        }

        public long TransactionId { get; set; }
        public string TxHash { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public string? Comment { get; set; }
        public long IsCreated { get; set; }
        public long? BlockHeight { get; set; }
        public long? FeeSatoshi { get; set; }

        public virtual ICollection<Input> Inputs { get; set; }
        public virtual ICollection<Output> Outputs { get; set; }
    }
}
