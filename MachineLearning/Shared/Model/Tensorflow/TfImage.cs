using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MachineLearning.Shared.Model.Tensorflow
{
    public class TfImageMetadata
    {
        public static int ImageHeight { get; set; } = 224;
        public static int ImageWidth { get; set; } = 224;
        public static float Mean { get; set; } = 117;
        public static bool ChannelsLast { get; set; } = true;
        [LoadColumn(0)]
        public string ImagePath;
        [LoadColumn(1)]
        public string Label;
        public static IEnumerable<TfImageMetadata> ReadFromCsv(string file, string folder, char seperator = ',')
        {
            return File.ReadAllLines(file)
             .Select(x => x.Split(seperator))
             .Select(x =>
             {
                 return new TfImageMetadata { ImagePath = Path.Combine(folder, x[0]), Label = x[1] };
             });
        }
    }
    public class TfImageDataProbability : TfImageMetadata
    {
        public string PredictedLabel { get; set; }
        public float Probability { get; set; }
        public override string ToString()
        {
            return $@"original is [{this.Label}] and predicted as [{PredictedLabel}] ({Probability}), {Path.GetFileNameWithoutExtension(ImagePath)}";
        }
    }
    public class TfImagePredictionResult : TfImageMetadata
    {
        [ColumnName(Constants.Tensorflow.TfImage.outputTensorName)]
        public float[] PredictedLabels;
        public TfImageDataProbability Translate(string[] labels)
        {
            var max = PredictedLabels.Max();
            var index = PredictedLabels.AsSpan().IndexOf(max);
            return new TfImageDataProbability()
            {
                PredictedLabel = labels[index],
                Probability = max,
                Label = this.Label,
                ImagePath = this.ImagePath
            };
        }
    }
}
