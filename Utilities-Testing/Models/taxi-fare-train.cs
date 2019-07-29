using System;
using Utilities.Attributes.SQL;

[Table("[taxi-fare-train]")]
public class taxifaretrain
{
    public string vendor_id { get; set; }
    public double rate_code { get; set; }
    public double passenger_count { get; set; }
    public double trip_time_in_secs { get; set; }
    public double trip_distance { get; set; }
    public string payment_type { get; set; }
    public double fare_amount { get; set; }
}

