using MachineLearning.Shared.Model.Tensorflow;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning
{
    public static class TensorFlow
    {
        public static PredictionEngine<TfImageMetadata, TfImagePredictionResult> ImageClassification(MLContext context, IDataView data, string tensorflowModelLocation, string imagesFolder)
        {
            var pipeline = context.Transforms.LoadImages(outputColumnName: "input", imageFolder: imagesFolder, inputColumnName: nameof(TfImageMetadata.ImagePath)).
                     Append(context.Transforms.ResizeImages(outputColumnName: "input", imageWidth: TfImageMetadata.ImageWidth, imageHeight: TfImageMetadata.ImageHeight, inputColumnName: "input")).
                     Append(context.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: TfImageMetadata.ChannelsLast, offsetImage: TfImageMetadata.Mean)).
                     Append(context.Model.LoadTensorFlowModel(tensorflowModelLocation).ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true));
            var model = pipeline.Fit(data);
            var predictionEngine = context.Model.CreatePredictionEngine<TfImageMetadata, TfImagePredictionResult>(model);
            return predictionEngine;
        }
        public static PredictionEngine<TfImageMetadata, TfImagePredictionResult> ImageClassification(IEnumerable<TfImageMetadata> tfImages, string tensorflowModelLocation, string imagesFolder)
        {
            var context = new MLContext();
            var data = context.Data.LoadFromEnumerable(tfImages);
            return ImageClassification(context, data, tensorflowModelLocation, imagesFolder);
        }
        public static PredictionEngine<TfImageMetadata, TfImagePredictionResult> ImageClassification(string dataLocation, string tensorflowModelLocation, string imagesFolder, bool hasHeader = false)
        {
            var context = new MLContext();
            var data = context.Data.LoadFromTextFile<TfImageMetadata>(path: dataLocation, hasHeader: hasHeader);
            return ImageClassification(context, data, tensorflowModelLocation, imagesFolder);
        }
    }
}
