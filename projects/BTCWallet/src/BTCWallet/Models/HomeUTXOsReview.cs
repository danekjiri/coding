using System;

namespace BTCWallet.Models;

public class HomeUTXOReview
{
    public DateTime DateCreated { get; set; }
    public string Address { get; set; } = null!;
    public string? Comment { get; set; }
    public long Amount { get; set; }
}