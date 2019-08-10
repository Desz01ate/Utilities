using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using MachineLearning.Shared.Attributes;
using MachineLearning.Shared.Model;
using Microsoft.ML.Data;
using System;
using System.Data.Common;
using Utilities.Attributes.SQL;

namespace MachineLearning.Examples.POCO
{
    public class TaxiFare
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
        [ColumnName("Label")]
        public float fare_amount { get; set; }
    }
    [Table("[taxi-fare-train]")]

    public class TaxiFareTrain : TaxiFare
    {

    }
    [Table("[taxi-fare-test]")]
    public class TaxiFareTest : TaxiFare
    {

    }
    public class TaxiFareRegression : RegressionPredictionResult
    {

    }
}
