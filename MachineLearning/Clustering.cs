using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MachineLearning.Shared;

namespace MachineLearning
{
    public static class Clustering
    {
        public static PredictionEngine<TIn, TOut> KMeans<TIn, TOut>(IEnumerable<TIn> trainDataset, int numberOfClusters = 5, string exampleWeightColumnName = null, Action<ITransformer> additionModelAction = null, params string[] excludedColumns)
        where TIn : class, new()
        where TOut : class, new()
        {
            var context = new MLContext();
            //var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => !excludedColumns.Contains(property)).ToArray();
            var properties = typeof(TIn).GetProperties().Where(x=>!excludedColumns.Contains(x.Name)).ToArray();
            var preprocessing = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            //var pipeline = context.Transforms.Concatenate("Features", features).
            //               Append(context.Clustering.Trainers.KMeans("Features", numberOfClusters: numberOfClusters, exampleWeightColumnName: exampleWeightColumnName));
            var pipeline = context.Transforms.Concatenate("Features", preprocessing.CombinedFeatures.ToArray())
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
                .Append(preprocessing.OneHotEncodingEstimator)
                .Append(context.Clustering.Trainers.KMeans(featureColumnName: "Features", numberOfClusters: numberOfClusters));

            var model = pipeline.Fit(trainDataframe);
            var engine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return engine;
        }
    }
}
