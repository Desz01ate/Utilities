using Microsoft.Data.DataView;
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
        public class CombinedFeature
        {
            public string Feature { get; set; }
            public string EncodedFeature { get; set; }
        }
        public class Featurization
        {
            public IEnumerable<CombinedFeature> Features { get; set; }
            public IEnumerable<string> CombinedFeatures { get; set; }
        }
        public static void SaveModel(this ITransformer model, string path)
        {
            var context = new MLContext();
            using (var fs = File.Create(path))
            {
                context.Model.Save(model, fs);
            }
        }
        public static ITransformer LoadModel(string path)
        {
            var context = new MLContext();
            using (var fs = File.Open(path, FileMode.Open))
            {
                var model = context.Model.Load(fs);
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
        public static RegressionMetrics EvaluateRegressionModel(ITransformer model, IDataView testDataframe)
        {
            var prediction = model.Transform(testDataframe);
            var metrics = new MLContext().Regression.Evaluate(prediction, label: DefaultColumnNames.Label, score: DefaultColumnNames.Score);
            return metrics;
        }
        public static MultiClassClassifierMetrics EvaluateMulticlassClassifierMetrics(ITransformer model, IDataView testDataframe)
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
        public static Featurization FeaturesCleaning(IEnumerable<PropertyInfo> properties, string encodedFormat = "{0}_encoded")
        {
            var featurization = new Featurization();
            var features = properties.Where(property => property.PropertyType != typeof(string)).Select(property => property.Name);
            var needToEncodeFeatures = properties.Where(property => property.PropertyType == typeof(string)).Select(property => property.Name);

            List<CombinedFeature> combinedFeatures = new List<CombinedFeature>();
            foreach (var feature in needToEncodeFeatures)
            {
                var encoded = string.Format(encodedFormat, feature);
                combinedFeatures.Add(new CombinedFeature
                {
                    Feature = feature,
                    EncodedFeature = encoded
                });
            }
            featurization.Features = combinedFeatures;
            featurization.CombinedFeatures = Shared.Enumerator.CombineEnumerator(features, combinedFeatures.Select(x => x.EncodedFeature));
            return featurization;
        }
    }
}
