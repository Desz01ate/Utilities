using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;

namespace MachineLearning
{
    public static class MulticlassClassfication
    {
        /// <summary>
        /// Create engine of Limited-Memory Broyden–Fletcher–Goldfarb–Shanno algorithm using training dataset and hyperparameters
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
        /// <param name="historySize">Memory size. Low=faster, less accurate.</param>
        /// <param name="enforceNonNegativity">Enforce non-negative weights.</param>
        /// <returns></returns>
        public static PredictionEngine<TIn, TOut> LbfgsMaximumEntropy<TIn, TOut>(IEnumerable<TIn> trainDataset, string outputColumnName = "PredictedLabel", string exampleWeightColumnName = null, float l1Regularization = 1, float l2Regularization = 1, double optimizationTolerance = 1e-07, int historySize = 20, bool enforceNonNegativity = false, Action<ITransformer> additionModelAction = null)
            where TIn : class, new()
            where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
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
        public static PredictionEngine<TIn, TOut> SdcaMaximumEntropy<TIn, TOut>(IEnumerable<TIn> trainDataset, string outputColumnName = "PredictedLabel", string exampleWeightColumnName = null, ISupportSdcaClassificationLoss loss = null, float? l2Regularization = null, float? l1Regularization = null, int? maximumNumberOfIterations = null, Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);


            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l2Regularization: l2Regularization,
                    l1Regularization: l1Regularization,
                    maximumNumberOfIterations: maximumNumberOfIterations
                ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> NaiveBayes<TIn, TOut>(IEnumerable<TIn> trainDataset, string outputColumnName = "PredictedLabel", Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
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
    public static class BinaryClassification
    {
        public static PredictionEngine<TIn, TOut> FastTree<TIn, TOut>(IEnumerable<TIn> trainDataset, string outputColumnName = "PredictedLabel", Action<ITransformer> additionModelAction = null)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
                .AppendCacheCheckpoint(context)
                .Append(context.BinaryClassification.Trainers.FastTree(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features"
                 ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> FastForest<TIn, TOut>(IEnumerable<TIn> trainDataset, string outputColumnName = "PredictedLabel", Action<ITransformer> additionModelAction = null)
where TIn : class, new()
where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
                .AppendCacheCheckpoint(context)
                .Append(context.BinaryClassification.Trainers.FastForest(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features"
                 ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> SdcaLogisticRegression<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string outputColumnName = "PredictedLabel",
            string exampleWeightColumnName = null,
            float? l1Regularization = null,
            float? l2Regularization = null,
            int? maximumNumberOfIterations = null,
            Action<ITransformer> additionModelAction = null)
        where TIn : class, new()
        where TOut : class, new()
        {
            var context = new MLContext();
            var type = typeof(TIn);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);

            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);
            var pipeline = context.Transforms.Conversion.MapValueToKey(labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .AppendCacheCheckpoint(context)
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: labelColumnName,
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l1Regularization: l1Regularization,
                    l2Regularization: l2Regularization,
                    maximumNumberOfIterations: maximumNumberOfIterations
                 ))
                .Append(context.Transforms.Conversion.MapKeyToValue(outputColumnName));
            var model = pipeline.Fit(trainDataframe);
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }

    }
}
