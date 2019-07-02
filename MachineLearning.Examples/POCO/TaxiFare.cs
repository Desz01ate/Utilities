﻿using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using MachineLearning.Shared.Attributes;
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
        [IgnoreField(true, false)]
        public float rate_code { get; set; }
        [IgnoreField(true, false)]
        public float passenger_count { get; set; }
        [IgnoreField(true, false)]
        [MinMaxScaleColumn]
        public float trip_time_in_secs { get; set; }
        [IgnoreField(true, false)]
        public float trip_distance { get; set; }
        [IgnoreField(true, false)]
        [OneHotEncodingColumn]
        public string payment_type { get; set; }
        [IgnoreField(true, false)]
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
    public class TaxiFareRegression : PredictionRegressionModel<TaxiFare>
    {

    }
}
