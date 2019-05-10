using MachineLearning.Examples.Interfaces;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.Classes
{
    public abstract class PredictionRegressionModel<T> where T : class, IConstraint, new()
    {
        [ColumnName("Score")]
        public float Predicted_Score;
        [ColumnName("PCAFeatures")]
        public float[] Location { get; set; }
        [ColumnName("LastName")]
        public string LastName { get; set; }
        public virtual float CalculateVariance(T obj, int precision = 0)
        {
            return (float)Math.Round(Predicted_Score / obj.actual_float_result() * 100, precision);
        }
    }
    public abstract class PredictionMulticlassClassificationModel<T> where T : class, IConstraint, new()
    {
        [ColumnName("PredictedLabel")]
        public string Predicted_result { get; set; }
        [ColumnName("Score")]
        public float[] Distance { get; set; }
        [ColumnName("PCAFeatures")]
        public float[] Location { get; set; }
        [ColumnName("LastName")]
        public string LastName { get; set; }
        public virtual string ComparePrediction(T obj)
        {
            return obj.actual_string_result() == Predicted_result ? "Equals" : "Not Equals";
        }
    }
    public abstract class PredictionBinaryClassficationModel<T> where T : class, IConstraint, new()
    {
        public string Predicted_result { get; set; }
        public float Score { get; set; }
        public float Probability { get; set; }
        public string PredictedLabel { get; set; }
    }
    public abstract class PredictionClusteringModel<T> where T : class, IConstraint, new()
    {
        [ColumnName("PredictedLabel")]
        public uint Predicted_cluster;
        [ColumnName("Score")]
        public float[] Distance;
        [ColumnName("PCAFeatures")]
        public float[] Location;
        [ColumnName("LastName")]
        public string LastName;
    }
}
