using System;

namespace Test.Models
{
public class taxifaretrain
{
    public string vendor_id { get; set; }
    public int rate_code { get; set; }
    public int passenger_count { get; set; }
    public int trip_time_in_secs { get; set; }
    public double trip_distance { get; set; }
    public string payment_type { get; set; }
    public double fare_amount { get; set; }
}
}
