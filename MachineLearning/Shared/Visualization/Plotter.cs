using MachineLearning.Shared.Interface;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearning.Shared.Visualization
{
    public static class Plotter
    {
        private static void Clustering<T>(IEnumerable<T> predictions, string plotLocation, Func<T, object> selector) where T : class, IClusteringModel
        {
            //var plot = new PlotModel { Title = "Customer Segmentation", IsLegendVisible = true };

            //var clusters = predictions.Select(selector).Distinct().OrderBy(x => x);

            //foreach (var cluster in clusters)
            //{
            //    var scatter = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerStrokeThickness = 2, Title = $"Cluster: {cluster}", RenderInLegend = true };
            //    var series = predictions
            //        .Where(p => p.clusterId == cluster)
            //        .Select(p => new ScatterPoint(p.Location[0], p.Location[1])).ToArray();
            //    scatter.Points.AddRange(series);
            //    plot.Series.Add(scatter);
            //}

            //plot.DefaultColors = OxyPalettes.HueDistinct(plot.Series.Count).Colors;

            //var exporter = new SvgExporter { Width = 600, Height = 400 };
            //using (var fs = new System.IO.FileStream(plotLocation, System.IO.FileMode.Create))
            //{
            //    exporter.Export(plot, fs);
            //}

            //Console.WriteLine($"Plot location: {plotLocation}");
        }
    }
}
