
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
using Microsoft.ML.Trainers;
using System.Collections.Generic;
/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using System.Linq;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
After:
using System.Linq;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using System.Linq;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
After:
using System.Linq;
*/


namespace MachineLearning
{
    public static class Recommendation
    {
        public static PredictionEngine<TIn, TOut> MatrixFactorization<TIn, TOut>(IEnumerable<TIn> trainDataset, string rowIndexColumnName, string columnIndexColumnName, int iteration = 20, int approximationRank = 100, double learningRate = 0.2, MatrixFactorizationTrainer.LossFunctionType lossFunctionType = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass, bool suppressDetail = true, bool forceNonNegative = true, params string[] excludedColumns)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessing = Preprocessing.KeyToValueMapping(context, properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = $@"{columnIndexColumnName}_encoded",
                MatrixRowIndexColumnName = $@"{rowIndexColumnName}_encoded",
                LabelColumnName = "Label",
                NumberOfIterations = iteration,
                ApproximationRank = approximationRank,
                LearningRate = learningRate,
                LossFunction = lossFunctionType,
                Quiet = suppressDetail,
                NonNegative = forceNonNegative
            };


            var pipeline = preprocessing.KeyToValueMappingEstimator.Append(context.Recommendation().Trainers.MatrixFactorization(options));

            var model = pipeline.Fit(trainDataframe);
            var engine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return engine;
        }
    }
}
