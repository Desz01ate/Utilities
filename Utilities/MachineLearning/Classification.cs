using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.MachineLearning
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
        /// <param name="l1Weight">Weight of L1 regularization term.</param>
        /// <param name="l2Weight">Weight of L2 regularization term.</param>
        /// <param name="optimizationTolerance">Threshold for optimizer convergence.</param>
        /// <param name="memorySize">Memory size for Microsoft.ML.Trainers.MulticlassLogisticRegression. Low=faster, less accurate.</param>
        /// <param name="enforceNoNegative">Enforce non-negative weights.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> LogisticRegression<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string outputColumnName, string exampleWeightColumnName = null, float l1Weight = 1, float l2Weight = 1, double optimizationTolerance = 1e-07, int memorySize = 20, bool enforceNoNegative = false)
            where TIn : class, new()
            where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => property != labelColumnName).ToArray();
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(context.Transforms.Concatenate("Features", features))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.LogisticRegression(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l1Weight: l1Weight,
                    l2Weight: l2Weight,
                    optimizationTolerance: (float)optimizationTolerance,
                    memorySize: memorySize,
                    enforceNoNegativity: enforceNoNegative
                    )
                 )
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return predictEngine;
        }
        /// <summary>
        /// Create engine of Logistic Regression algorithm using training dataset and hyperparameters (recommend to use IEnumerable overload instead for explicit data cleaning)
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="dataPath">Path to data.</param>
        /// <param name="labelColumnName">The name of the label column.</param>
        /// <param name="outputColumnName">The name of the feature column.</param>
        /// <param name="seperator">Data seperator.</param>
        /// <param name="hasHeader">Is data contain header row.</param>
        /// <param name="exampleWeightColumnName">The name of the example weight column.</param>
        /// <param name="l1Weight">Weight of L1 regularization term.</param>
        /// <param name="l2Weight">Weight of L2 regularization term.</param>
        /// <param name="optimizationTolerance">Threshold for optimizer convergence.</param>
        /// <param name="memorySize">Memory size for Microsoft.ML.Trainers.MulticlassLogisticRegression. Low=faster, less accurate.</param>
        /// <param name="enforceNoNegative">Enforce non-negative weights.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> LogisticRegression<TIn, TOut>(string dataPath, string labelColumnName, string outputColumnName, char seperator = '\t', bool hasHeader = false, string exampleWeightColumnName = null, float l1Weight = 1, float l2Weight = 1, double optimizationTolerance = 1e-07, int memorySize = 20, bool enforceNoNegative = false)
            where TIn : class, new()
            where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => property != labelColumnName).ToArray();
            var trainDataframe = context.Data.LoadFromTextFile<TIn>(dataPath, seperator, hasHeader);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(context.Transforms.Concatenate("Features", features))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.LogisticRegression(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l1Weight: l1Weight,
                    l2Weight: l2Weight,
                    optimizationTolerance: (float)optimizationTolerance,
                    memorySize: memorySize,
                    enforceNoNegativity: enforceNoNegative
                    )
                 )
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
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
        /// <param name="loss">The optional custom loss.</param>
        /// <param name="l2Const">The L2 regularization hyperparameter.</param>
        /// <param name="l1Threshold">The L1 regularization hyperparameter. Higher values will tend to lead to more sparse model.</param>
        /// <param name="maxIterations">The maximum number of passes to perform over the data.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> StochasticDualCoordinateAscent<TIn, TOut>(IEnumerable<TIn> trainDataset, string labelColumnName, string outputColumnName, string exampleWeightColumnName = null, ISupportSdcaClassificationLoss loss = null, float? l2Const = null, float? l1Threshold = null, int? maxIterations = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => property != labelColumnName).ToArray();
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(context.Transforms.Concatenate("Features", features))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l2Const: l2Const,
                    l1Threshold: l1Threshold,
                    maxIterations: maxIterations,
                    loss: loss
                ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return predictEngine;
        }
        /// <summary>
        /// Create engine of Stochastic Dual Coordination Ascent (optimization as such Stochastic Gradient Descent) algorithm using training dataset and hyperparameters (recommend to use IEnumerable overload instead for explicit data cleaning)
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="dataPath">Path to data.</param>
        /// <param name="labelColumnName">The name of the label column.</param>
        /// <param name="outputColumnName">The name of the feature column.</param>
        /// <param name="seperator">Data seperator.</param>
        /// <param name="hasHeader">Is data contain header row.</param>
        /// <param name="exampleWeightColumnName">The name of the example weight column.</param>
        /// <param name="loss">The optional custom loss.</param>
        /// <param name="l2Const">The L2 regularization hyperparameter.</param>
        /// <param name="l1Threshold">The L1 regularization hyperparameter. Higher values will tend to lead to more sparse model.</param>
        /// <param name="maxIterations">The maximum number of passes to perform over the data.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> StochasticDualCoordinateAscent<TIn, TOut>(string dataPath, string labelColumnName, string outputColumnName, char seperator = '\t', bool hasHeader = false, string exampleWeightColumnName = null, ISupportSdcaClassificationLoss loss = null, float? l2Const = null, float? l1Threshold = null, int? maxIterations = null)
            where TIn : class, new()
            where TOut : class, new()
        {
            var context = new MLContext();
            var features = typeof(TIn).GetProperties().Select(property => property.Name).Where(property => property != labelColumnName).ToArray();
            var trainDataframe = context.Data.LoadFromTextFile<TIn>(dataPath, seperator, hasHeader);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(context.Transforms.Concatenate("Features", features))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l2Const: l2Const,
                    l1Threshold: l1Threshold,
                    maxIterations: maxIterations,
                    loss: loss
                ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            return predictEngine;
        }

    }

}
