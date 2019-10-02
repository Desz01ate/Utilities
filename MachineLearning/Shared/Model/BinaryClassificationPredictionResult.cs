using Microsoft.ML.Data;
using System;

namespace MachineLearning.Shared.Model
{
    public class BinaryClassificationPredictionResult
    {
        [ColumnName(Constants.IDataView.BinaryClassification.Label)]
        public bool Label { get; set; }
        [ColumnName(Constants.IDataView.BinaryClassification.Score)]
        public float Score { get; set; }
        [ColumnName(Constants.IDataView.BinaryClassification.Probability)]
        public float Probability { get; set; }
        [ColumnName(Constants.IDataView.BinaryClassification.PredictedLabel)]
        public bool PredictedLabel { get; set; }
        [ColumnName(Constants.IDataView.BinaryClassification.PCAFeatures)]
        public float[] PCAFeatures { get; set; }
        [Obsolete("Unverified column name.")]
        [ColumnName(Constants.IDataView.BinaryClassification.LastName)]
        public string LastName { get; set; }
    }
}
