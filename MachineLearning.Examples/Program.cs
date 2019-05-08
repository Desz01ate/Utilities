using MachineLearning.Examples.POCO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearning.Examples
{

    //this class implement the internal code of  Microsoft.ML.Trainers.SquaredLoss to illustrate how to implement custom loss function.
    class CustomSquaredLoss : ISupportSdcaRegressionLoss
    {
        public float ComputeDualUpdateInvariant(float scaledFeaturesNormSquared)
        {
            return 1 / ((float)0.5 + scaledFeaturesNormSquared);
        }

        public float Derivative(float output, float label)
        {
            float diff = output - label;
            return 2 * diff;
        }

        public double DualLoss(float label, float dual)
        {
            return -dual * (dual / 4 - label);
        }

        public float DualUpdate(float output, float label, float dual, float invariant, int maxNumThreads)
        {
            var fullUpdate = (label - output - (float)0.5 * dual) * invariant;
            return maxNumThreads >= 2 ? fullUpdate / maxNumThreads : fullUpdate;
        }

        public double Loss(float output, float label)
        {
            float diff = output - label;
            return diff * diff;
        }
    }
    class Program
    {
        class StudentExam
        {
            [LoadColumn(0)]
            public float Q1 { get; set; }
            [LoadColumn(1)]
            public float Q2 { get; set; }
            [LoadColumn(2)]
            public float Q3 { get; set; }
            [LoadColumn(3)]
            public float Result { get; set; }
        }
        class StudentExamPredict
        {
            [ColumnName("Score")]
            public float result { get; set; }
            public float CalculateVariance(StudentExam std)
            {
                return (float)Math.Round(Math.Abs(result / std.Result) * 100);
            }
        }
        static async Task Main(string[] args)
        {
            //var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            //var train = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Sales>(sqlConnection, "SELECT * FROM [Sales]");
            //var testdata = new Sales()
            //{
            //    productId = "263",
            //    month = 10,
            //    year = 2017,
            //    avg = 91,
            //    max = 370,
            //    min = 1,
            //    count = 10,
            //    prev = 1675,
            //    units = 910
            //};

            //var context = new MLContext();
            //var dataframe = context.Data.LoadFromEnumerable(train);
            //var pipeline = context.Transforms.Concatenate(outputColumnName: "numerical_features",
            //                        nameof(Sales.year), nameof(Sales.month), nameof(Sales.units), nameof(Sales.avg), nameof(Sales.count),
            //                        nameof(Sales.max), nameof(Sales.min), nameof(Sales.prev))
            //                .Append(context.Transforms.Categorical.OneHotEncoding(outputColumnName: "country_encoded", nameof(Sales.productId)))
            //                .Append(context.Transforms.Concatenate("Features", "numerical_features", "country_encoded"))
            //                .Append(context.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(Sales.next)))
            //                .Append(context.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features"));

            //var crossValidation = context.Regression.CrossValidate(data: dataframe, estimator: pipeline, numberOfFolds: 6, labelColumnName: "Label");
            //var model = pipeline.Fit(dataframe);
            //var engine = context.Model.CreatePredictionEngine<Sales, SalesPrediction>(model);
            //var engine2 = MachineLearning.Regression.FastTree<Sales, SalesPrediction>(train, nameof(Sales.next), learningRate: 0.2, numberOfLeaves: 40,numberOfTrees:200);

            //Console.WriteLine(engine.Predict(testdata).Score);
            //Console.WriteLine(engine2.Predict(testdata).Score);

            //Console.ReadLine();
            await MulticlassClassificationExample();
            await RegressionExample();
        }
        static async Task MulticlassClassificationExample()
        {
            var bestAlg = string.Empty;
            double logLoss = double.MaxValue;
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Wine>(sqlConnection, "SELECT * FROM [wine]");
            var trainSize = (int)(traindata.Count() * 0.8);
            var testdata = traindata.Skip(trainSize);
            traindata = traindata.Take(trainSize);


            var algorithms = new Dictionary<string, Func<IEnumerable<Wine>, string, Action<ITransformer>, PredictionEngine<Wine, WinePrediction>>>() {
                { "SdcaMaximumEntropy", (data,label,action) => MulticlassClassfication.SdcaMaximumEntropy<Wine,WinePrediction>(data,label,additionModelAction:action) },
                { "LbfgsMaximumEntropy", (data,label,action) => MulticlassClassfication.LbfgsMaximumEntropy<Wine,WinePrediction>(data,label,additionModelAction:action) },
                { "NaiveBayes", (data,label,action) => MulticlassClassfication.NaiveBayes<Wine,WinePrediction>(data,label,additionModelAction:action) },
            };
            foreach (var algorithm in algorithms)
            {
                var engine = algorithm.Value(traindata, nameof(Wine.type), (model) =>
                {
                    MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for {algorithm.Key} algorithm.");
                    var metrics = Global.EvaluateMulticlassClassificationMetrics(model, mlContext.Data.LoadFromEnumerable(testdata), labelColumnName: nameof(Wine.type));
                    foreach (var prop in metrics.GetType().GetProperties())
                    {
                        Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                    }
                    if (metrics.LogLoss < logLoss)
                    {
                        logLoss = metrics.LogLoss;
                        bestAlg = algorithm.Key;
                    }
                });
                foreach (var t in testdata.Take(20))
                {
                    var predict = engine.Predict(t);
                    Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5}", t.type, predict.type));
                } 
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on Log Loss : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static async Task RegressionExample()
        {
            var bestAlg = string.Empty;
            double mse = double.MaxValue;
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            var testdata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT TOP(10) * FROM [taxi-fare-test]");
            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT * FROM [taxi-fare-train]");
            var algorithms = new Dictionary<string, Func<IEnumerable<TaxiFare>, string, Action<ITransformer>, PredictionEngine<TaxiFare, TaxiFarePrediction>>>() {
                { "SDCA", (data,label,action) => Regression.StochasticDoubleCoordinateAscent<TaxiFare,TaxiFarePrediction>(data,label,additionModelAction : action) },
                { "LBFGS", (data,label,action) => Regression.LbfgsPoisson<TaxiFare,TaxiFarePrediction>(data,label,additionModelAction : action) },
                { "FastTree", (data,label,action) => Regression.FastTree<TaxiFare,TaxiFarePrediction>(data,label,additionModelAction : action) },
                { "FastTreeTweedie", (data,label,action) => Regression.FastTreeTweedie<TaxiFare,TaxiFarePrediction>(data,label,additionModelAction : action) },
                { "FastForest", (data,label,action) => Regression.FastForest<TaxiFare,TaxiFarePrediction>(data,label,additionModelAction : action) },
            };
            foreach (var algorithm in algorithms)
            {
                var engine = algorithm.Value(traindata, nameof(TaxiFare.fare_amount), (model) =>
                {
                    var metrics = Global.EvaluateRegressionModel(model, mlContext.Data.LoadFromEnumerable(testdata));
                    MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for {algorithm.Key} algorithm.");
                    foreach (var prop in metrics.GetType().GetProperties())
                    {
                        Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                    }
                    if (metrics.MeanSquaredError < mse)
                    {
                        mse = metrics.MeanSquaredError;
                        bestAlg = algorithm.Key;
                    }
                });
                foreach (var t in testdata.Take(20))
                {
                    var predict = engine.Predict(t);
                    Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5} ({2,0}%)", Math.Round(t.fare_amount, 2), Math.Round(predict.FareAmount, 2), predict.CalculateVariance(t)));
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on Mean Squared Error : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
