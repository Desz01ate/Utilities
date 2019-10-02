using Utilities.Attributes.SQL;

namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    [Utilities.Attributes.SQL.Table("[taxi-fare-test]")]
    public class taxifaretest
    {
        public string vendor_id { get; set; }
        public float rate_code { get; set; }
        public float passenger_count { get; set; }
        public float trip_time_in_secs { get; set; }
        public float trip_distance { get; set; }
        public string payment_type { get; set; }
        [Field("fare_amount")]
        public float fareamount { get; set; }
    }
}