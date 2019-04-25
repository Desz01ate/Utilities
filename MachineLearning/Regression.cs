using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearning
{
    public static class Regression
    {
        public static PredictionEngine<TIn, TOut> FastTree<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string exampleWeightColumnName = null, int numLeaves = 20, int numTrees = 100, int minDatapointsInLeaves = 10, double learningRate = 0.2, Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var properties = typeof(TIn).GetProperties();

            var features = Global.FeaturesCleaning(properties);
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);

            EstimatorChain<OneHotEncodingTransformer> oneHotEncodingEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            foreach (var feature in features.Features)
            {
                oneHotEncodingEstimator = oneHotEncodingEstimator.Append(context.Transforms.Categorical.OneHotEncoding(inputColumnName: feature.Feature, outputColumnName: feature.EncodedFeature));
            }

            var pipeline = context.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: labelColumnName)
                .Append(oneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", features.CombinedFeatures.ToArray()))
                .Append(context.Regression.Trainers.FastTree(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    numLeaves: numLeaves,
                    numTrees: numTrees,
                    minDatapointsInLeaves: minDatapointsInLeaves,
                    learningRate: learningRate
                ));

            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }


    }
}
