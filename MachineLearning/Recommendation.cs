using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;

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
            var properties = type.GetProperties().Where(property =>
            {
                var attributes = property.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is ExcludeColumn excludeColumn) return false;
                }
                return true;
            });


            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var preprocessor = context.ValueToKeyMapping(properties);

            var estimator = preprocessor.ValueToKeyMappingEstimator;
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

            var pipeline = estimator.Append(context.Recommendation().Trainers.MatrixFactorization(options));

            var model = pipeline.Fit(trainDataframe);
            var engine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return engine;
        }
    }
}
