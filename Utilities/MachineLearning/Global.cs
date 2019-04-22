using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities.MachineLearning
{
    public static class Global
    {
        public static void SaveModel(this ITransformer model, Stream stream)
        {
            var context = new MLContext();
            context.Model.Save(model, stream);
        }
        public static RegressionMetrics EvaluateRegressionModel(ITransformer model, IDataView testDataframe)
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().Regression.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            return metrics;
        }
        public static MultiClassClassifierMetrics EvaluateMulticlassClassifierMetrics(ITransformer model,IDataView testDataframe)
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().MulticlassClassification.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            return metrics;
        }
        public static BinaryClassificationMetrics EvaluateBinaryClassificationMetrics(ITransformer model, IDataView testDataframe)
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().BinaryClassification.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            return metrics;
        }
    }
}
