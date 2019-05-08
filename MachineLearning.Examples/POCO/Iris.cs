using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.POCO
{
    public class Iris
    {
        [LoadColumn(0)]
        public float SepalLength { get; set; }

        [LoadColumn(1)]
        public float SepalWidth { get; set; }

        [LoadColumn(2)]
        public float PetalLength { get; set; }

        [LoadColumn(3)]
        public float PetalWidth { get; set; }
        [LoadColumn(4)]
        public string Label { get; set; }
    }
    public class IrisPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Label { get; set; }
    }
    public class IrisClustering
    {
        [ColumnName("PredictedLabel")]
        public uint Label { get; set; }
    }
}
