using System;
using Utilities.Attributes.SQL;

namespace Utilities.Testing.Models
{
    [Table("[taxi-fare-test]")]
    public class taxifaretest
    {
        [Field("vendor_id")]
        [PrimaryKey]
        public string v { get; set; }
        public float rate_code { get; set; }
        public float passenger_count { get; set; }
        public float trip_time_in_secs { get; set; }
        public float trip_distance { get; set; }
        public string payment_type { get; set; }
        public float fare_amount { get; set; }
    }
}
