using System;
using System.Collections.Generic;

namespace BTCWallet.DataModels
{
    public partial class Output
    {
        public long OutputId { get; set; }
        public long TransactionId { get; set; }
        public long? AddressId { get; set; }
        public string AddressWif { get; set; } = null!;
        public long AmountSatoshi { get; set; }
        public long OutputIndex { get; set; }
        public string ScriptPubKey { get; set; } = null!;
        public long? IsSpent { get; set; }
        public long? InputId { get; set; }

        public virtual Address? Address { get; set; }
        public virtual Input? Input { get; set; }
        public virtual Transaction Transaction { get; set; } = null!;
    }
}
