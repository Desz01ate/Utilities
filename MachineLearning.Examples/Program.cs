using MachineLearning.Examples.POCO;
using Microsoft.ML;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MachineLearning.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var sqlConnection = $@"Server = localhost;database = test;user = sa;password = sa";
            var data = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Wine>(sqlConnection, "SELECT * FROM [wine]");
            var trainSize = (int)(data.Count() * 0.8);
            var traindata = data.Take(trainSize);
            var testdata = data.Skip(trainSize);
            var scda = Classification.SdcaMaximumEntropy<Wine, WinePrediction>(traindata, "type", "PredictedLabel");
            //var logisticRegression = Classification.LogisticRegression<Wine, WinePrediction>(traindata, "type", "PredictedLabel", additionModelAction: (model) =>
            //{
            //    Global.SaveModel(model, "LR.zip");
            //});
            var testDataframe = new MLContext().Data.LoadFromEnumerable(testdata);
            var lrModel = MachineLearning.Global.LoadModel("LR.zip");
            //var metrics = Utilities.MachineLearning.Global.EvaluateMulticlassClassifierMetrics(lrModel, testDataframe);
            //Console.WriteLine(metrics.LogLoss);
            var logisticRegression = MachineLearning.Global.CreatePredictionEngine<Wine, WinePrediction>(lrModel);
            var correctness1 = 0;
            var correctness2 = 0;
            foreach (var _data in testdata)
            {
                var nbResult = scda.Predict(_data);
                var nbCorrection = nbResult.type == _data.type;
                Console.ForegroundColor = nbCorrection ? ConsoleColor.Green : ConsoleColor.Red;
                correctness1 += Convert.ToInt32(nbCorrection);
                Console.WriteLine($@"Naive Bayes - Actual : {_data.type}, Predicted : {nbResult.type}");
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
    }
}
