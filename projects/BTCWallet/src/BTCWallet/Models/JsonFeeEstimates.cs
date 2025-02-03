using Newtonsoft.Json;

namespace BTCWallet.Models;

public class TotalDetails
{
    [JsonProperty("usd")]
    public double Usd { get; set; }

    [JsonProperty("satoshi")]
    public double Satoshi { get; set; }
}

public class Total
{
    [JsonProperty("p2wpkh")]
    // ReSharper disable once InconsistentNaming
    public TotalDetails? P2WPK { get; set; }

    [JsonProperty("p2sh-p2wpkh")]
    // ReSharper disable once InconsistentNaming
    public TotalDetails? P2SHP2WPKH { get; set; }

    [JsonProperty("p2pkh")]
    // ReSharper disable once InconsistentNaming
    public TotalDetails? P2PKH { get; set; }
}

public class EstimatesDetails
{
    [JsonProperty("sat_per_vbyte")]
    public double SatPerVbyte { get; set; }

    [JsonProperty("total")]
    public Total? Total { get; set; }
}

public class Estimates
{
    [JsonProperty("30")]
    public EstimatesDetails? Thirty { get; set; }

    [JsonProperty("60")]
    public EstimatesDetails? Sixty { get; set; }

    [JsonProperty("120")]
    public EstimatesDetails? OneTwenty { get; set; }

    [JsonProperty("180")]
    public EstimatesDetails? OneEighty { get; set; }

    [JsonProperty("360")]
    public EstimatesDetails? ThreeSixty { get; set; }

    [JsonProperty("720")]
    public EstimatesDetails? SevenTwenty { get; set; }

    [JsonProperty("1440")]
    public EstimatesDetails? FourteenForty { get; set; }
}

public class Root
{
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    [JsonProperty("estimates")]
    public Estimates? Estimates { get; set; }
}
