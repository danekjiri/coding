using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Input
    {
        public Input()
        {
            Outputs = new HashSet<Output>();
        }

        public long InputId { get; set; }
        public long TransactionId { get; set; }
        public long InputIndex { get; set; }
        public long AmountSatoshi { get; set; }
        public string? ScriptSig { get; set; }
        public string PreviousOutTxHash { get; set; } = null!;
        public long PreviousOutIndex { get; set; }

        public virtual Transaction Transaction { get; set; } = null!;
        public virtual ICollection<Output> Outputs { get; set; }
    }
}
