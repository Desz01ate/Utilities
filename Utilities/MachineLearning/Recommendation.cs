using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.MachineLearning
{
    public static class Recommendation
    {
        public static PredictionEngine<TIn, TOut> MatrixFactorization<TIn, TOut>(IEnumerable<TIn> trainDataset, int numberOfClusters = 5, string exampleWeightColumnName = null, params string[] excludedColumns)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => !excludedColumns.Contains(property)).ToArray();
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Concatenate("Features", features).
                           Append(context.Clustering.Trainers.KMeans("Features", clustersCount: numberOfClusters, exampleWeightColumnName: exampleWeightColumnName));
            var model = pipeline.Fit(trainDataframe);
            var engine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return engine;
        }
    }
}
