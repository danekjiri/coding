using System;

namespace BTCWallet.Models;

public class WalletProfileMenuItem(string menuLabel, Type menuPageType)
{
    public string MenuLabel { get; } = menuLabel;
    public Type ModelType { get; } = menuPageType;
}