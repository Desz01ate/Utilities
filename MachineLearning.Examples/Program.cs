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
            await ClassificationExample();
            await RegressionExample();
        }
        static async Task ClassificationExample()
        {

            var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            var data = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Wine>(sqlConnection, "SELECT * FROM [wine]");
            var trainSize = (int)(data.Count() * 0.8);
            var traindata = data.Take(trainSize);
            var testdata = data.Skip(trainSize);
            var scda = Classification.SdcaMaximumEntropy<Wine, WinePrediction>(traindata, "type");
            var logisticRegression = Classification.LbfgsMaximumEntropy<Wine, WinePrediction>(traindata, "type");


            var correctness1 = 0;
            var correctness2 = 0;
            foreach (var _data in testdata)
            {
                var nbResult = scda.Predict(_data);
                var nbCorrection = nbResult.type == _data.type;
                Console.ForegroundColor = nbCorrection ? ConsoleColor.Green : ConsoleColor.Red;
                correctness1 += Convert.ToInt32(nbCorrection);
                Console.WriteLine($@"SCDA - Actual : {_data.type}, Predicted : {nbResult.type}");
                Console.ForegroundColor = ConsoleColor.White;
                var lrResult = logisticRegression.Predict(_data);
                var lrCorrection = lrResult.type == _data.type;
                Console.ForegroundColor = lrCorrection ? ConsoleColor.Green : ConsoleColor.Red;
                correctness2 += Convert.ToInt32(lrCorrection);
                Console.WriteLine($@"Logistic Regression - Actual : {_data.type}, Predicted : {lrResult.type}");
                Console.ForegroundColor = ConsoleColor.White;
                //Console.WriteLine($@"Score : {string.Join(" ", result.score)}");
            }
            Console.WriteLine($@"SCDA Accuracy : {((float)correctness1 / testdata.ToList().Count) * 100}%");
            Console.WriteLine($@"Logistic Regression Accuracy : {((float)correctness2 / testdata.ToList().Count) * 100}%");
        }
        static async Task RegressionExample()
        {
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            var testdata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT TOP(3000) * FROM [taxi-fare-test]");
            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT TOP(7000) * FROM [taxi-fare-train]");
            var sdca = MachineLearning.Regression.StochasticDoubleCoordinateAscent<TaxiFare, TaxiFarePrediction>(traindata, nameof(TaxiFare.fare_amount),lossFunction : new CustomSquaredLoss(), additionModelAction: (model) =>
            {
                var metrics = Global.EvaluateRegressionModel(model, mlContext.Data.LoadFromEnumerable(testdata));
                MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for SDCA algorithm.");
                foreach (var prop in metrics.GetType().GetProperties())
                {
                    Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                }
            });
            foreach (var t in testdata.Take(20))
            {
                var predict = sdca.Predict(t);
                Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5} ({2,0}%)", Math.Round(t.fare_amount, 2), Math.Round(predict.FareAmount, 2), predict.CalculateVariance(t)));
            }
            var lbfgs = MachineLearning.Regression.LbfgsPoisson<TaxiFare, TaxiFarePrediction>(traindata, nameof(TaxiFare.fare_amount), additionModelAction: (model) =>
            {
                var metrics = Global.EvaluateRegressionModel(model, mlContext.Data.LoadFromEnumerable(testdata));
                MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for L-BFGS algorithm.");
                foreach (var prop in metrics.GetType().GetProperties())
                {
                    Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                }
            });
            foreach (var t in testdata.Take(20))
            {
                var predict = lbfgs.Predict(t);
                Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5} ({2,0}%)", Math.Round(t.fare_amount, 2), Math.Round(predict.FareAmount, 2), predict.CalculateVariance(t)));
            }
        }
    }
}
