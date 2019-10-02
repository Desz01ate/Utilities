using Microsoft.ML;
using Microsoft.ML.Data;

namespace MachineLearning.Shared
{
    public static class Metrics
    {
        public static RegressionMetrics EvaluateRegressionModel(ITransformer model, IDataView testDataframe, string labelColumnName = "Label", string scoreColumnName = "Score")
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().Regression.Evaluate(prediction, labelColumnName, scoreColumnName);
            return metrics;
        }
        public static BinaryClassificationMetrics EvaluateBinaryClassificationMetrics(ITransformer model, IDataView testDataframe, string labelColumnName = "Label", string scoreColumnName = "Score", string probabilityColumnName = "Probability", string predictedLabelColumnName = "PredictedLabel")
        {
            var prediction = model.Transform(testDataframe);
            var mlContext = new MLContext();
            try
            {
                var metrics = mlContext.BinaryClassification.Evaluate(prediction, labelColumnName, scoreColumnName, probabilityColumnName, predictedLabelColumnName);
                return metrics;
            }
            catch
            {
                var metrics = mlContext.BinaryClassification.EvaluateNonCalibrated(prediction, labelColumnName, scoreColumnName, predictedLabelColumnName);
                return metrics;
            }
        }
        public static MulticlassClassificationMetrics EvaluateMulticlassClassificationMetrics(ITransformer model, IDataView testDataframe, string labelColumnName = "Label", string scoreColumnName = "Score", string predictedLabelColumnName = "PredictedLabel")
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().MulticlassClassification.Evaluate(prediction, labelColumnName, scoreColumnName, predictedLabelColumnName);
            return metrics;
        }
        public static ClusteringMetrics EvaluateClusteringMetrics(ITransformer model, IDataView testDataframe, string labelColumnName = null, string scoreColumnName = "Score", string featureColumnName = null)
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().Clustering.Evaluate(prediction, labelColumnName: labelColumnName, scoreColumnName: scoreColumnName, featureColumnName: featureColumnName);
            return metrics;
        }
    }
}
