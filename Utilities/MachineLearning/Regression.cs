using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.MachineLearning
{
    public static class Regression
    {
        public static PredictionEngine<TIn, TOut> FastTree<TIn, TOut>(IEnumerable<TIn> trainDataset, string inputColumnName)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var properties = typeof(TIn).GetProperties();
            var features = properties.Where(property => property.PropertyType != typeof(string)).Select(property => property.Name).Where(property => property != inputColumnName).ToArray();
            var needToEncodeFeatures = properties.Where(property => property.PropertyType == typeof(string)).Select(property => property.Name);
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);

            EstimatorChain<OneHotEncodingTransformer> oneHotEncodingEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            List<string> encodedFeaturesName = new List<string>();
            foreach (var feature in needToEncodeFeatures)
            {
                var encoded = $@"{feature}_encoded";
                oneHotEncodingEstimator = oneHotEncodingEstimator.Append(context.Transforms.Categorical.OneHotEncoding(outputColumnName: encoded, inputColumnName: feature));
                encodedFeaturesName.Add(encoded);
            }
            var completedFeatures = Shared.LINQ.CombineEnumerator(features, encodedFeaturesName);
            var pipeline = context.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: inputColumnName)
                .Append(oneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", completedFeatures))
                .Append(context.Regression.Trainers.FastTree());
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return predictEngine;
        }
    }
}
