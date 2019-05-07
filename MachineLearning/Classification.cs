using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearning
{
    public static class Classification
    {
        /// <summary>
        /// Create engine of Logistic Regression algorithm using training dataset and hyperparameters
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="trainDataset">Enumerable of TIn type.</param>
        /// <param name="labelColumnName">The name of the label column.</param>
        /// <param name="outputColumnName">The name of the feature column.</param>
        /// <param name="exampleWeightColumnName">The name of the example weight column.</param>
        /// <param name="l1Regularization">Weight of L1 regularization term.</param>
        /// <param name="l2Regularization">Weight of L2 regularization term.</param>
        /// <param name="optimizationTolerance">Threshold for optimizer convergence.</param>
        /// <param name="historySize">Memory size for Microsoft.ML.Trainers.MulticlassLogisticRegression. Low=faster, less accurate.</param>
        /// <param name="enforceNonNegativity">Enforce non-negative weights.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> LogisticRegression<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string outputColumnName, string exampleWeightColumnName = null, float l1Regularization = 1, float l2Regularization = 1, double optimizationTolerance = 1e-07, int historySize = 20, bool enforceNonNegativity = false, Action<ITransformer> additionModelAction = null)
            where TIn : class, new()
            where TOut : class, new()
        {
            var context = new MLContext();
            var properties = typeof(TIn).GetProperties().Where(property => property.Name != labelColumnName);
            var features = Global.FeaturesCleaning(properties);

            EstimatorChain<OneHotEncodingTransformer> oneHotEncodingEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            foreach (var feature in features.Features)
            {
                oneHotEncodingEstimator = oneHotEncodingEstimator.Append(context.Transforms.Categorical.OneHotEncoding(inputColumnName: feature.Feature, outputColumnName: feature.EncodedFeature));
            }

            //var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => property != labelColumnName).ToArray();
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(oneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", features.CombinedFeatures.ToArray()))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l1Regularization: l1Regularization,
                    l2Regularization: l2Regularization,
                    optimizationTolerance: (float)optimizationTolerance,
                    historySize: historySize,
                    enforceNonNegativity: enforceNonNegativity
                    )
                 )
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));


            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        /// <summary>
        /// Create engine of Stochastic Dual Coordination Ascent (optimization as such Stochastic Gradient Descent) algorithm using training dataset and hyperparameters
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="trainDataset">Enumerable of TIn type.</param>
        /// <param name="labelColumnName">The name of the label column.</param>
        /// <param name="outputColumnName">The name of the feature column.</param>
        /// <param name="exampleWeightColumnName">The name of the example weight column.</param>
        /// <param name="l2Regularization">The L2 regularization hyperparameter.</param>
        /// <param name="l1Regularization">The L1 regularization hyperparameter. Higher values will tend to lead to more sparse model.</param>
        /// <param name="maximumNumberOfIterations">The maximum number of passes to perform over the data.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> SdcaMaximumEntropy<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string outputColumnName, string exampleWeightColumnName = null, ISupportSdcaClassificationLoss loss = null, float? l2Regularization = null, float? l1Regularization = null, int? maximumNumberOfIterations = null, Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var properties = typeof(TIn).GetProperties().Where(property => property.Name != labelColumnName);
            var features = Global.FeaturesCleaning(properties);

            EstimatorChain<OneHotEncodingTransformer> oneHotEncodingEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            foreach (var feature in features.Features)
            {
                oneHotEncodingEstimator = oneHotEncodingEstimator.Append(context.Transforms.Categorical.OneHotEncoding(inputColumnName: feature.Feature, outputColumnName: feature.EncodedFeature));
            }

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(oneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", features.CombinedFeatures.ToArray()))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l2Regularization : l2Regularization,
                    l1Regularization : l1Regularization,
                    maximumNumberOfIterations : maximumNumberOfIterations
                ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> NaiveBayes<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string outputColumnName, Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var properties = typeof(TIn).GetProperties().Where(property => property.Name != labelColumnName);
            var features = Global.FeaturesCleaning(properties);

            EstimatorChain<OneHotEncodingTransformer> oneHotEncodingEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            foreach (var feature in features.Features)
            {
                oneHotEncodingEstimator = oneHotEncodingEstimator.Append(context.Transforms.Categorical.OneHotEncoding(inputColumnName: feature.Feature, outputColumnName: feature.EncodedFeature));
            }

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(oneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", features.CombinedFeatures.ToArray()))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.NaiveBayes(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features"
                 ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
    }
}
