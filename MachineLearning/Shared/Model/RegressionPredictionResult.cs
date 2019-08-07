using MachineLearning.Shared.Model.Interfaces;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.Model
{
    public class RegressionPredictionResult
    {
        [ColumnName(Constants.IDataView.Regression.Label)]
        public float Label { get; set; }
        [ColumnName(Constants.IDataView.Regression.Score)]
        public float Score { get; set; }
        [ColumnName(Constants.IDataView.Regression.PCAFeatures)]
        public float[] PCAFeatures { get; set; }
        [Obsolete("Unverified column name.")]
        [ColumnName(Constants.IDataView.Regression.LastName)]
        public string LastName { get; set; }
        public virtual float CalculateDifferential(int precision = 0)
        {
            return (float)System.Math.Round(Score / Label, precision);
        }
    }
}
