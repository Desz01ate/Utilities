using Microsoft.ML.Data;

namespace MachineLearning.Examples.POCO
{
    public class TaxiFare
    {
        [LoadColumn(0)]
        public string vendor_id { get; set; }
        [LoadColumn(1)]
        public float rate_code { get; set; }
        [LoadColumn(2)]
        public float passenger_count { get; set; }
        [LoadColumn(3)]
        public float trip_time_in_secs { get; set; }
        [LoadColumn(4)]
        public float trip_distance { get; set; }
        [LoadColumn(5)]
        public string payment_type { get; set; }
        [LoadColumn(6)]
        public float fare_amount { get; set; }

    }
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}
