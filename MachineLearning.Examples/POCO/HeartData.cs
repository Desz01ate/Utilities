using MachineLearning.Shared.Attributes;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.POCO
{
    public class HeartData
    {
        [LoadColumn(0)]
        public float Age { get; set; }
        [LoadColumn(1)]
        public float Sex { get; set; }
        [LoadColumn(2)]
        public float Cp { get; set; }
        [LoadColumn(3)]
        public float TrestBps { get; set; }
        [LoadColumn(4)]
        public float Chol { get; set; }
        [LoadColumn(5)]
        public float Fbs { get; set; }
        [LoadColumn(6)]
        public float RestEcg { get; set; }
        [LoadColumn(7)]
        public float Thalac { get; set; }
        [LoadColumn(8)]
        public float Exang { get; set; }
        [LoadColumn(9)]
        public float OldPeak { get; set; }
        [LoadColumn(10)]
        public float Slope { get; set; }
        [LoadColumn(11)]
        public float Ca { get; set; }
        [LoadColumn(12)]
        public float Thal { get; set; }
        [LabelColumn]
        [LoadColumn(13)]
        public bool Label { get; set; }
    }
    public class HeartPredict
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public bool Prediction;

        // No need to specify ColumnName attribute, because the field
        // name "Probability" is the column name we want.
        public float Probability;

        public float Score;
    }
}
