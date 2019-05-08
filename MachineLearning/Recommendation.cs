using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using MachineLearning.Shared;

namespace MachineLearning
{
    public static class Recommendation
    {
        public static PredictionEngine<TIn, TOut> MatrixFactorization<TIn, TOut>(IEnumerable<TIn> trainDataset, string rowIndexColumnName, string columnIndexColumnName, int iteration = 20, int approximationRank = 100, double learningRate = 0.2, MatrixFactorizationTrainer.LossFunctionType lossFunctionType = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass, bool suppressDetail = true, bool forceNonNegative = true, params string[] excludedColumns)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().ToArray();
            if (excludedColumns != null)
            {
                features = features.Where(property => !excludedColumns.Contains(property.Name)).ToArray();
            }
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var preprocessor = context.ValueToKeyMapping(features);

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

            //var pipeline = estimator.Append(context.Recommendation().Trainers.MatrixFactorization(matrixColumnIndexColumnName: $@"", matrixRowIndexColumnName: $@"", labelColumn: "Label", settings));
            var pipeline = estimator.Append(context.Recommendation().Trainers.MatrixFactorization(options));

            var model = pipeline.Fit(trainDataframe);
            var engine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return engine;
        }
    }
}
