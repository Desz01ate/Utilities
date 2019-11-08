
/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using Microsoft.ML;
After:
using MachineLearning.ML;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using Microsoft.ML;
After:
using MachineLearning.ML;
*/
using MachineLearning.Shared;
using Microsoft.ML;

/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using Microsoft.ML.Transforms;
After:
using Microsoft.ML.Data;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using Microsoft.ML.Transforms;
After:
using Microsoft.ML.Data;
*/
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System;
using System.Linq;
/* Unmerged change from project 'MachineLearning (netstandard2.1)'
Before:
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
using Microsoft.ML.Trainers.FastTree;
After:
using System;
using System.Collections.Generic;
using System.Linq;
*/

/* Unmerged change from project 'MachineLearning (net461)'
Before:
using MachineLearning.Shared;
using MachineLearning.Shared.Attributes;
using Microsoft.ML.Trainers.FastTree;
After:
using System;
using System.Collections.Generic;
using System.Linq;
*/


namespace MachineLearning
{
    public static class Regression
    {
        /// <summary>
        /// A base template of regression trainer which contains pre-processing likes OHE,PCA with any choosing algorithm.
        /// </summary>
        /// <typeparam name="TType">Type of training data.</typeparam>
        /// <typeparam name="TTrainer">Type of trainer algorithm.</typeparam>
        /// <param name="context">Microsoft.ML context.</param>
        /// <param name="trainDataset">Training dataset.</param>
        /// <param name="estimator">Algorithm estimator.</param>
        /// <returns>Model of training datatype from given estimator.</returns>
        private static TransformerChain<TTrainer> RegressionTrainerTemplate<TType, TTrainer>(this MLContext context, IEnumerable<TType> trainDataset, IEstimator<TTrainer> estimator)
        where TType : class, new()
        where TTrainer : class, ITransformer
        {
            var type = typeof(TType);
            var labelColumnName = Preprocessing.LabelColumn(type.GetProperties()).Name;
            var properties = Preprocessing.ExcludeColumns(type.GetProperties());

            var preprocessor = context.OneHotEncoding(properties);
            var trainDataframe = context.Data.LoadFromEnumerable(trainDataset);

            var pipeline = context.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: labelColumnName)
                .Append(preprocessor.OneHotEncodingEstimator)
                .Append(context.Transforms.Concatenate("Features", preprocessor.CombinedFeatures.ToArray()))
                .Append(context.Transforms.ProjectToPrincipalComponents(outputColumnName: "PCAFeatures", inputColumnName: "Features", rank: 2))
                .Append(estimator);
            var model = pipeline.Fit(trainDataframe);
            return model;
        }
        public static PredictionEngine<TIn, TOut> StochasticDoubleCoordinateAscent<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            ISupportSdcaRegressionLoss lossFunction = null,
            float? l1Regularization = null,
            float? l2Regularization = null,
            int? maximumNumberOfIterations = null,
            Action<ITransformer> additionModelAction = null)
    where TIn : class, new()
    where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.Sdca(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    lossFunction: lossFunction,
                    l1Regularization: l1Regularization,
                    l2Regularization: l2Regularization,
                    maximumNumberOfIterations: maximumNumberOfIterations
                ));
            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> LbfgsPoisson<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            float l1Regularization = 1,
            float l2Regularization = 1,
            float optimizationTolerance = (float)1E-07,
            int historySize = 20,
            bool enforceNonNegativity = false,
            Action<ITransformer> additionModelAction = null
            ) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.LbfgsPoissonRegression(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    l1Regularization: l1Regularization,
                    l2Regularization: l2Regularization,
                    optimizationTolerance: optimizationTolerance,
                    historySize: historySize,
                    enforceNonNegativity: enforceNonNegativity
                    ));

            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> FastTreeTweedie<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            int numberOfLeaves = 20,
            int numberOfTrees = 100,
            int minimumExampleCountPerLeaft = 10,
            double learningRate = 0.2,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.FastTreeTweedie(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    numberOfLeaves: numberOfLeaves,
                    numberOfTrees: numberOfTrees,
                    minimumExampleCountPerLeaf: minimumExampleCountPerLeaft,
                    learningRate: learningRate
                    ));

            var predictEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictEngine;
        }
        public static PredictionEngine<TIn, TOut> FastTree<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            int numberOfLeaves = 20,
            int numberOfTrees = 100,
            int minimumExampleCountPerLeaft = 10,
            double learningRate = 0.2,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.FastTree(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    numberOfLeaves: numberOfLeaves,
                    numberOfTrees: numberOfTrees,
                    minimumExampleCountPerLeaf: minimumExampleCountPerLeaft,
                    learningRate: learningRate
                    ));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
        public static PredictionEngine<TIn, TOut> FastForest<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            int numberOfLeaves = 20,
            int numberOfTrees = 100,
            int minimumExampleCountPerLeaft = 10,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.FastForest(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: exampleWeightColumnName,
                    numberOfLeaves: numberOfLeaves,
                    numberOfTrees: numberOfTrees,
                    minimumExampleCountPerLeaf: minimumExampleCountPerLeaft
                    ));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
        public static PredictionEngine<TIn, TOut> GeneralizedAdditiveModel<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            string exampleWeightColumnName = null,
            int numberOfIterations = 9500,
            int maximumBinCountPerFeature = 255,
            double learningRate = 0.002,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.Gam(
                labelColumnName: "Label",
                featureColumnName: "Features",
                exampleWeightColumnName,
                numberOfIterations,
                maximumBinCountPerFeature,
                learningRate
            ));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
        public static PredictionEngine<TIn, TOut> GeneralizedAdditiveModel<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            GamRegressionTrainer.Options options,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.Gam(options));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
        public static PredictionEngine<TIn, TOut> OnlineGradientDescent<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            IRegressionLoss lossFunction = null,
            float learningRate = 0.1f,
            bool decreaseLearningRate = true,
            float l2Regularization = 0,
            int numberOfIterations = 1,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.OnlineGradientDescent(
                 labelColumnName: "Label",
                 featureColumnName: "Features",
                 lossFunction,
                 learningRate,
                 decreaseLearningRate,
                 l2Regularization,
                 numberOfIterations
            ));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
        public static PredictionEngine<TIn, TOut> OnlineGradientDescent<TIn, TOut>(
            IEnumerable<TIn> trainDataset,
            OnlineGradientDescentTrainer.Options options,
            Action<ITransformer> additionModelAction = null) where TIn : class, new() where TOut : class, new()
        {
            var context = new MLContext();
            var model = context.RegressionTrainerTemplate(trainDataset, context.Regression.Trainers.OnlineGradientDescent(options));
            var predictionEngine = context.Model.CreatePredictionEngine<TIn, TOut>(model);
            additionModelAction?.Invoke(model);
            return predictionEngine;
        }
    }
}
