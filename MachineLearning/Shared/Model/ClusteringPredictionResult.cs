using Microsoft.ML.Data;
using System;

namespace MachineLearning.Shared.Model
{
    public class ClusteringPredictionResult
    {
        [ColumnName(Constants.IDataView.Clustering.Label)]
        public string Label { get; set; }
        [ColumnName(Constants.IDataView.Clustering.Score)]
        public float[] Score { get; set; }
        [ColumnName(Constants.IDataView.Clustering.PredictedLabel)]
        public uint PredictedLabel { get; set; }
        [ColumnName(Constants.IDataView.Clustering.PCAFeatures)]
        public float[] PCAFeatures { get; set; }
        [Obsolete("Unverified column name.")]
        [ColumnName(Constants.IDataView.Clustering.LastName)]
        public string LastName { get; set; }
    }
}
