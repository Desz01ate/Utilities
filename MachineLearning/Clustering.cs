
/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using Microsoft.ML;
After:
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
using Microsoft.ML;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using Microsoft.ML;
After:
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
using Microsoft.ML;
*/
using MachineLearning.Shared;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using System.Text;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
After:
using System.Text;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using System.Text;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
After:
using System.Text;
*/


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
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

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
