using MachineLearning.Shared.Model.Interfaces;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.Model
{
    public class MulticlassClassficationPredictionResult
    {
        [ColumnName(Constants.IDataView.MulticlassClassification.Label)]
        public uint Label { get; set; }
        [ColumnName(Constants.IDataView.MulticlassClassification.Score)]
        public float[] Score { get; set; }
        [ColumnName(Constants.IDataView.MulticlassClassification.PredictedLabel)]
        public string PredictedLabel { get; set; }
        [ColumnName(Constants.IDataView.MulticlassClassification.PCAFeatures)]
        public float[] PCAFeatures { get; set; }
        [Obsolete("Unverified column name.")]
        [ColumnName(Constants.IDataView.MulticlassClassification.LastName)]
        public string LastName { get; set; }
    }
}
