using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using Microsoft.ML.Data;
using System;

namespace MachineLearning.Examples.POCO
{
    public class TaxiFare : IConstraint
    {
        public string vendor_id { get; set; }
        public float rate_code { get; set; }
        public float passenger_count { get; set; }
        public float trip_time_in_secs { get; set; }
        public float trip_distance { get; set; }
        public string payment_type { get; set; }
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
    public class TaxiFareRegression : PredictionRegressionModel<TaxiFare>
    {

    }
}
