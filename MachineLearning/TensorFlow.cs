using MachineLearning.Shared.Model.Tensorflow;
using Microsoft.ML;
using System.Collections.Generic;

namespace MachineLearning
{
    public static class TensorFlow
    {
        /// <summary>
        /// Scores a dataset using a pre-trained TensorFlow model.
        /// </summary>
        /// <param name="context">MLContext instant</param>
        /// <param name="data">Data to train</param>
        /// <param name="tensorflowModelLocation">Tensorflow pre-trained model (*.pb).</param>
        /// <param name="imagesFolder">Images folder to load into trainer</param>
        /// <returns></returns>
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
        /// <summary>
        /// Scores a dataset using a pre-trained TensorFlow model.
        /// </summary>
        /// <param name="tfImages">IEnumerable of TfImageMetadata</param>
        /// <param name="tensorflowModelLocation">Tensorflow pre-trained model (*.pb).</param>
        /// <param name="imagesFolder">Images folder to load into trainer</param>
        /// <returns></returns>
        public static PredictionEngine<TfImageMetadata, TfImagePredictionResult> ImageClassification(IEnumerable<TfImageMetadata> tfImages, string tensorflowModelLocation, string imagesFolder)
        {
            var context = new MLContext();
            var data = context.Data.LoadFromEnumerable(tfImages);
            return ImageClassification(context, data, tensorflowModelLocation, imagesFolder);
        }
        /// <summary>
        /// Scores a dataset using a pre-trained TensorFlow model.
        /// </summary>
        /// <param name="dataLocation">Data path to load as TfImageMetadata</param>
        /// <param name="tensorflowModelLocation">Tensorflow pre-trained model (*.pb).</param>
        /// <param name="imagesFolder">Images folder to load into trainer</param>
        /// <param name="hasHeader">Is dataLocation file has header row.</param>
        /// <returns></returns>
        public static PredictionEngine<TfImageMetadata, TfImagePredictionResult> ImageClassification(string dataLocation, string tensorflowModelLocation, string imagesFolder, bool hasHeader = false)
        {
            var context = new MLContext();
            var data = context.Data.LoadFromTextFile<TfImageMetadata>(path: dataLocation, hasHeader: hasHeader);
            return ImageClassification(context, data, tensorflowModelLocation, imagesFolder);
        }
    }
}
