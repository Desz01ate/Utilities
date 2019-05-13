using MachineLearning.Shared.Attributes;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.POCO
{
    class XOR
    {
        public float x { get; set; }
        public float y { get; set; }
        [LabelColumn]
        public bool R { get; set; }
    }
    class XORPredict : XOR
    {
        [ColumnName("PredictedLabel")]
        public bool Predicted_result { get; set; }
        [ColumnName("Score")]
        public float Distance { get; set; }
        [ColumnName("PCAFeatures")]
        public float[] Location { get; set; }
        [ColumnName("LastName")]
        public string LastName { get; set; }
        [ColumnName("Probability")]
        public float Prob { get; set; }
    }
}
