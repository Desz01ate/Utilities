using MachineLearning.Examples.POCO;
using MachineLearning.Shared;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
        class Test : Interfaces.IConstraint
        {
            public string key { get; set; }
            public float value { get; set; }
            public float v1 { get; set; }
            public float v2 { get; set; }
            public float v3 { get; set; }
            public float v4 { get; set; }
            public float v5 { get; set; }

            public float actual_float_result()
            {
                return value;
            }

            public string actual_string_result()
            {
                return key;
            }

            public uint actual_uint_result()
            {
                throw new NotImplementedException();
            }
        }
        class TestPredict : Classes.PredictionRegressionModel<Test>
        {

        }
        static async Task Main(string[] args)
        {
            bool train = true;
            //await BinaryClassifier(train);
            //await MulticlassClassificationExample(train);
            //await RegressionExample(train);
            //await ClusteringExample(train);
            var sqlConnection = $@"Server = localhost;database = Local;user = sa;password = sa";
            var traindata = Utilities.SQL.SQLServer.ExecuteReader(sqlConnection, "SELECT * FROM [taxi-fare-train]", parameters: null, objectBuilder: (row) => Utilities.Shared.Data.RowBuilderExplicit<TaxiFare>(row));
            var obj = traindata.ToList()[0];
            var test = Utilities.SQL.SQLServer.Select<TaxiFare>(sqlConnection);
            foreach (var t in traindata)
            {
                Utilities.SQL.SQLServer.Insert(sqlConnection, t);
            }
            var sql = Utilities.SQL.SQLServer.Update(sqlConnection, obj);
            Console.WriteLine(sql);
            sql = Utilities.SQL.SQLServer.Delete(sqlConnection, obj);
            Console.WriteLine(sql);
            Console.ReadLine();
        }
        static async Task BinaryClassifier(bool train = true)
        {
            var sqlConnection = $@"Server = localhost;database = Local;user = sa;password = sa";

            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<HeartData>(sqlConnection, $@"SELECT * FROM HeartTraining");
            var testdata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<HeartData>(sqlConnection, $@"SELECT * FROM HeartTest");

            double accuracy = double.MinValue;
            string bestAlg = string.Empty;
            var mlContext = new MLContext();
            var algorithms = new Dictionary<string, Func<IEnumerable<HeartData>, Action<ITransformer>, PredictionEngine<HeartData, HeartPredict>>>() {
                { "FastTree", (data,action) => BinaryClassification.FastTree<HeartData,HeartPredict>(data, additionModelAction:action) },
                { "FastForest", (data,action) => BinaryClassification.FastForest<HeartData,HeartPredict>(data,additionModelAction:action) },
                { "SdcaLogisticRegression", (data,action) => BinaryClassification.SdcaLogisticRegression<HeartData,HeartPredict>(data,additionModelAction:action) },
            };
            foreach (var algorithm in algorithms)
            {
                PredictionEngine<HeartData, HeartPredict> engine = default;
                ITransformer model = default;
                var path = $@"BClassification_{algorithm.Key}.zip";
                if (File.Exists(path) && !train)
                {
                    model = MachineLearning.Global.LoadModel(path);
                    engine = mlContext.Model.CreatePredictionEngine<HeartData, HeartPredict>(model);

                }
                else
                {
                    engine = algorithm.Value(traindata, (mdl) =>
                    {
                        model = mdl;
                    });
                }
                MachineLearning.Global.SaveModel(model, $@"Binaryclass_{algorithm.Key}.zip");
                MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for {algorithm.Key} algorithm.");
                try
                {
                    var metrics = Metrics.EvaluateBinaryClassificationMetrics(model, mlContext.Data.LoadFromEnumerable(testdata), labelColumnName: nameof(HeartData.Label));
                    foreach (var prop in metrics.GetType().GetProperties())
                    {
                        Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                    }
                    if (metrics.Accuracy > accuracy)
                    {
                        accuracy = metrics.Accuracy;
                        bestAlg = algorithm.Key;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to evaluate metrics : {e.Message}");
                }

                foreach (var t in testdata)
                {
                    var predict = engine.Predict(t);
                    Console.WriteLine(string.Format(@"Actual {0,5} / Predict {1,5} with prob of {2,5}", t.Label, predict.Prediction, predict.Probability));
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on accuracy : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static async Task MulticlassClassificationExample(bool train = true)
        {
            var bestAlg = string.Empty;
            double logLoss = double.MaxValue;
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = Local;user = sa;password = sa";
            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Iris>(sqlConnection, "SELECT * FROM [Iris] ORDER BY NEWID()");
            var testdata = traindata.Take(20);



            var algorithms = new Dictionary<string, Func<IEnumerable<Iris>, Action<ITransformer>, PredictionEngine<Iris, IrisClassification>>>() {
                { "SdcaMaximumEntropy", (data,action) => MulticlassClassfication.SdcaMaximumEntropy<Iris,IrisClassification>(data,additionModelAction:action) },
                { "LbfgsMaximumEntropy", (data,action) => MulticlassClassfication.LbfgsMaximumEntropy<Iris,IrisClassification>(data,additionModelAction:action) },
                { "NaiveBayes", (data,action) => MulticlassClassfication.NaiveBayes<Iris,IrisClassification>(data,additionModelAction:action) },
            };
            foreach (var algorithm in algorithms)
            {
                PredictionEngine<Iris, IrisClassification> engine = default;
                ITransformer model = default;
                var path = $@"MClassification_{algorithm.Key}.zip";
                if (File.Exists(path) && !train)
                {
                    model = MachineLearning.Global.LoadModel(path);
                    engine = mlContext.Model.CreatePredictionEngine<Iris, IrisClassification>(model);

                }
                else
                {
                    engine = algorithm.Value(traindata, (mdl) =>
                    {
                        model = mdl;
                    });
                }
                MachineLearning.Global.SaveModel(model, $@"Multiclass_{algorithm.Key}.zip");
                MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for {algorithm.Key} algorithm.");
                var metrics = Metrics.EvaluateMulticlassClassificationMetrics(model, mlContext.Data.LoadFromEnumerable(testdata), labelColumnName: nameof(Iris.Label));
                foreach (var prop in metrics.GetType().GetProperties())
                {
                    Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                }
                if (metrics.LogLoss < logLoss)
                {
                    logLoss = metrics.LogLoss;
                    bestAlg = algorithm.Key;
                }
                List<IrisClassification> irisClassifications = new List<IrisClassification>();
                foreach (var t in testdata)
                {
                    var predict = engine.Predict(t);
                    irisClassifications.Add(predict);
                    Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5} {2}", t.Label, predict.Predicted_result, predict.ComparePrediction(t)));
                }
                //VisualizeMulticlassClassification(algorithm.Key, testdata, irisClassifications, $"{algorithm.Key}_clsf.svg");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on Log Loss : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static async Task RegressionExample(bool train = true)
        {
            var bestAlg = string.Empty;
            double mse = double.MaxValue;
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = Local;user = sa;password = sa";
            var testdata = (await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT TOP(10) * FROM [taxi-fare-test]", parameters: null, objectBuilder: (row) => Utilities.Shared.Data.RowBuilderExplicit<TaxiFare>(row)));
            var traindata = (await Utilities.SQL.SQLServer.ExecuteReaderAsync<TaxiFare>(sqlConnection, "SELECT * FROM [taxi-fare-train] ORDER BY NEWID()", parameters: null, objectBuilder: (row) => Utilities.Shared.Data.RowBuilderExplicit<TaxiFare>(row)));
            var algorithms = new Dictionary<string, Func<IEnumerable<TaxiFare>, Action<ITransformer>, PredictionEngine<TaxiFare, TaxiFareRegression>>>() {
                { "SDCA", (data,action) => Regression.StochasticDoubleCoordinateAscent<TaxiFare,TaxiFareRegression>(data,additionModelAction : action) },
                { "LBFGS", (data,action) => Regression.LbfgsPoisson<TaxiFare,TaxiFareRegression>(data,additionModelAction : action) },
                { "FastTree", (data,action) => Regression.FastTree<TaxiFare,TaxiFareRegression>(data,additionModelAction : action) },
                { "FastTreeTweedie", (data,action) => Regression.FastTreeTweedie<TaxiFare,TaxiFareRegression>(data,additionModelAction : action) },
                { "FastForest", (data,action) => Regression.FastForest<TaxiFare,TaxiFareRegression>(data,additionModelAction : action) },
            };
            foreach (var algorithm in algorithms)
            {
                PredictionEngine<TaxiFare, TaxiFareRegression> engine = default;
                ITransformer model = default;
                var path = $@"Regression_{algorithm.Key}.zip";
                if (File.Exists(path) && !train)
                {
                    model = Global.LoadModel(path);
                    engine = mlContext.Model.CreatePredictionEngine<TaxiFare, TaxiFareRegression>(model);

                }
                else
                {
                    engine = algorithm.Value(traindata, (mdl) =>
                    {
                        model = mdl;
                    });
                }
                MachineLearning.Global.SaveModel(model, $@"Regression_{algorithm.Key}.zip");
                var metrics = Metrics.EvaluateRegressionModel(model, mlContext.Data.LoadFromEnumerable(testdata));
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
                //var predictedList = new List<TaxiFareRegression>();
                //foreach (var t in testdata)
                //{
                //    var predict = engine.Predict(t);
                //    predictedList.Add(predict);
                //    Console.WriteLine(string.Format(@"Actual : {0,5} / Predict {1,5} ({2,0}%)", Math.Round(t.fare_amount, 2), Math.Round(predict.Predicted_Score, 2), predict.CalculateVariance(t)));
                //}
                var predictedList = engine.Predict(testdata);
                VisualizeRegression(algorithm.Key, testdata, predictedList, metrics, $"{algorithm.Key}_reg.svg");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on Mean Squared Error : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static async Task ClusteringExample(bool train = true)
        {
            var bestAlg = string.Empty;
            double avgdist = double.MaxValue;
            var mlContext = new MLContext();
            var sqlConnection = $@"Server = localhost;database = Local;user = sa;password = sa";
            var traindata = await Utilities.SQL.SQLServer.ExecuteReaderAsync<Iris>(sqlConnection, "SELECT * FROM [iris] ORDER BY NEWID()");
            var trainSize = (int)(traindata.Count() * 0.8);
            var testdata = traindata.Skip(trainSize).ToList();
            traindata = traindata.Take(trainSize).ToList();


            var algorithms = new Dictionary<string, Func<IEnumerable<Iris>, Action<ITransformer>, PredictionEngine<Iris, IrisClustering>>>() {
                { "KMeans", (data,action) => Clustering.KMeans<Iris,IrisClustering>(data,3,additionModelAction:action) },
            };
            foreach (var algorithm in algorithms)
            {
                PredictionEngine<Iris, IrisClustering> engine = default;
                ITransformer model = default;
                var path = $@"Clustering_{algorithm.Key}.zip";
                if (File.Exists(path) && !train)
                {
                    model = MachineLearning.Global.LoadModel(path);
                    engine = mlContext.Model.CreatePredictionEngine<Iris, IrisClustering>(model);

                }
                else
                {
                    engine = algorithm.Value(traindata, (mdl) =>
                    {
                        model = mdl;
                    });
                }
                MachineLearning.Global.SaveModel(model, path);
                MachineLearning.ConsoleHelper.ConsoleWriteHeader($@"Evaluate metrics for {algorithm.Key} algorithm.");
                var dataframe = new MLContext().Data.LoadFromEnumerable(testdata);
                var metrics = Metrics.EvaluateClusteringMetrics(model, dataframe);
                foreach (var prop in metrics.GetType().GetProperties())
                {
                    Console.WriteLine($@"{prop.Name} : {prop.GetValue(metrics)}");
                }
                if (metrics.AverageDistance < avgdist)
                {
                    avgdist = metrics.AverageDistance;
                    bestAlg = algorithm.Key;
                }
                var predictedData = new List<IrisClustering>();
                foreach (var t in testdata)
                {
                    var temp = t.Label;
                    var predict = engine.Predict(t);
                    predictedData.Add(predict);
                    Console.WriteLine(string.Format(@"Cluster ID : {0,5}", predict.Predicted_cluster));
                }
                VisualizeClustering(predictedData, "clustering.svg");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Best algorithm based-on Average Distance : {bestAlg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void VisualizeMulticlassClassification(string algorithmName, IEnumerable<Wine> testData, IEnumerable<WineClassification> predictedData, string savePath)
        {
            try
            {
                var plot = new PlotModel { Title = "Iris Type Prediction", IsLegendVisible = true };
                var types = predictedData.Select(x => x.Predicted_result).Distinct().OrderBy(x => x);
                foreach (var type in types)
                {
                    var scatter = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerStrokeThickness = 2, Title = $"type : {type}" };
                    var series = predictedData.Where(x => x.Predicted_result == type).Select(p => new ScatterPoint(p.Location[0], p.Location[1]));
                    scatter.Points.AddRange(series);
                    plot.Series.Add(scatter);
                }
                plot.DefaultColors = OxyPalettes.HueDistinct(plot.Series.Count).Colors;
                var exporter = new SvgExporter { Width = 600, Height = 400 };
                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    exporter.Export(plot, fs);
                }
                Console.WriteLine($"Classification svg generated at {savePath}.");
            }
            catch
            {
                Console.WriteLine($"Unable to generate visualization for {algorithmName}");
            }
        }
        static void VisualizeClustering(IEnumerable<IrisClustering> predictedData, string savePath)
        {
            var plot = new PlotModel { Title = "Iris Cluster", IsLegendVisible = true };
            var clusters = predictedData.Select(x => x.Predicted_cluster).Distinct().OrderBy(x => x);
            foreach (var cluster in clusters)
            {
                var scatter = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerStrokeThickness = 2, Title = $"Cluster : {cluster}" };
                var series = predictedData.Where(x => x.Predicted_cluster == cluster).Select(p => new ScatterPoint(p.Location[0], p.Location[1]));
                scatter.Points.AddRange(series);
                plot.Series.Add(scatter);
            }
            plot.DefaultColors = OxyPalettes.HueDistinct(plot.Series.Count).Colors;
            var exporter = new SvgExporter { Width = 600, Height = 400 };
            using (var fs = new FileStream(savePath, FileMode.Create))
            {
                exporter.Export(plot, fs);
            }
            Console.WriteLine($"Clustering svg generated at {savePath}.");
        }
        static void VisualizeRegression(string algorithmName, IEnumerable<TaxiFare> testData, IEnumerable<TaxiFareRegression> predictedData, RegressionMetrics regressionMetrics, string savePath)
        {
            var plot = new PlotModel { Title = $"{algorithmName} - Taxi Fare Regression", IsLegendVisible = true };
            plot.Axes.Add(new LinearAxis()
            {
                Title = regressionMetrics.MeanSquaredError.ToString(),
                Position = AxisPosition.Bottom,

            });
            var lineActual = new LineSeries()
            {
                Title = "Actual Price",
                Color = OxyColors.Green
            };
            var linePredict = new LineSeries
            {
                Title = "Predicted Price",
                Color = OxyColors.Red
            };
            var testArray = testData.ToArray();
            for (var x = 0; x < testArray.Length; x++)
            {
                var test = testArray[x];
                lineActual.Points.Add(new DataPoint(x, test.fare_amount));
            }
            var predictArray = predictedData.ToArray();
            for (var x = 0; x < predictArray.Length; x++)
            {
                var predict = predictArray[x];
                linePredict.Points.Add(new DataPoint(x, predict.Predicted_Score));
            }
            plot.Series.Add(lineActual);
            plot.Series.Add(linePredict);

            var exporter = new SvgExporter { Width = 600, Height = 400 };
            using (var fs = new FileStream(savePath, FileMode.Create))
            {
                exporter.Export(plot, fs);
            }
            Console.WriteLine($"Regression svg generated at {savePath}.");
        }
    }
}
