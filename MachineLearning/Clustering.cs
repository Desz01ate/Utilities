using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;

namespace MachineLearning
{
    public static class Clustering
    {
        public static PredictionEngine<TIn, TOut> KMeans<TIn, TOut>(IEnumerable<TIn> trainDataset, int numberOfClusters = 5, string exampleWeightColumnName = null, Action<ITransformer> additionModelAction = null)
        where TIn : class, new()
        where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = type.GetProperties().Where(property =>
            {
                var attributes = property.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is LabelColumn labelColumn) return true;
                }
                return false;
            }).FirstOrDefault().Name;
            var properties = type.GetProperties().Where(property =>
            {
                var attributes = property.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is ExcludeColumn excludeColumn) return false;
                    if (attribute is LabelColumn labelColumn) return false;
                }
                return true;
            });

            var preprocessing = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);

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
