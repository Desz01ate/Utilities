using MachineLearning.Shared.DataStructures;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MachineLearning
{
    public static class Global
    {
        public static void SaveModel(this ITransformer model, string path)
        {
            var context = new MLContext();
            using (var fs = File.Create(path))
            {
                context.Model.Save(model, null, fs);
            }
        }
        public static ITransformer LoadModel(string path)
        {
            var context = new MLContext();
            using (var fs = File.Open(path, FileMode.Open))
            {
                var model = context.Model.Load(fs, out var schema);
                return model;
            }
        }
        public static PredictionEngine<TIn, TOut> CreatePredictionEngine<TIn, TOut>(this ITransformer model)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            return context.Model.CreatePredictionEngine<TIn, TOut>(model);
        }
        public static RegressionMetrics EvaluateRegressionModel(ITransformer model, IDataView testDataframe, string labelColumnName = "Label", string scoreColumnName = "Score")
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().Regression.Evaluate(prediction, labelColumnName, scoreColumnName);
            return metrics;
        }
        public static BinaryClassificationMetrics EvaluateBinaryClassificationMetrics(ITransformer model, IDataView testDataframe, string labelColumnName = "Label", string scoreColumnName = "Score", string probabilityColumnName = "Probability", string predictedLabelColumnName = "PredictedLabel")
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().BinaryClassification.Evaluate(prediction, labelColumnName, scoreColumnName, probabilityColumnName, predictedLabelColumnName);
            return metrics;
        }
        public static void PeekOnDataView(ITransformer model, IDataView dataView, int numberOfRows = 10)
        {
            ConsoleHelper.ConsoleWriteHeader($"Peek data in DataView : Show {numberOfRows} rows.");
            var transformedData = model.Transform(dataView);
            var rows = transformedData.Preview(numberOfRows);
            foreach (var row in rows.RowView)
            {
                var columnCollection = row.Values;
                var sb = new StringBuilder();
                foreach (var column in columnCollection)
                {
                    sb.Append($" | {column.Key}:{column.Value}");
                }
                Console.WriteLine(sb);
            }
        }
    }
    public static class ConsoleHelper
    {
        public static void ConsoleWriteHeader(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            var maxLength = lines.Select(x => x.Length).Max();
            Console.WriteLine(new string('#', maxLength));
            Console.ForegroundColor = defaultColor;
        }
    }
}
