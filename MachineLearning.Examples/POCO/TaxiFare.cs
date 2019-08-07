using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using MachineLearning.Shared.Attributes;
using MachineLearning.Shared.Model;
using System;
using Utilities.Attributes.SQL;

namespace MachineLearning.Examples.POCO
{
    [Table("[taxi-fare-train2]")]
    public class TaxiFare : IConstraint
    {
        [PrimaryKey]
        [Field("vendor_id")]
        [OneHotEncodingColumn]
        public string vendor_id { get; set; }
        public float rate_code { get; set; }
        public float passenger_count { get; set; }
        [MinMaxScaleColumn]
        public float trip_time_in_secs { get; set; }
        public float trip_distance { get; set; }
        [OneHotEncodingColumn]
        [IgnoreField(false, false)]
        public string payment_type { get; set; }
        [LabelColumn]
        public float fare_amount { get; set; }

        public string actual_string_result()
        {
            throw new NotImplementedException();
        }

        public float actual_float_result()
        {
            return fare_amount;
        }

        public uint actual_uint_result()
        {
            throw new NotImplementedException();
        }
    }
    public class TaxiFareRegression : RegressionPredictionResult
    {

    }
}
