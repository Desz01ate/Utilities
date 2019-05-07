using Microsoft.ML.Data;

namespace MachineLearning.Examples.POCO
{
    public class Wine
    {
        [LoadColumn(0)]
        public string type { get; set; }
        [LoadColumn(1)]
        public float alcohol { get; set; }
        [LoadColumn(2)]
        public float malic_acid { get; set; }
        [LoadColumn(3)]
        public float ash { get; set; }
        [LoadColumn(4)]
        public float alcilinity_of_ash { get; set; }
        [LoadColumn(5)]
        public float magnesium { get; set; }
        [LoadColumn(6)]
        public float total_phenols { get; set; }
        [LoadColumn(7)]
        public float flavanoids { get; set; }
        [LoadColumn(8)]
        public float nonflavanoid_phenols { get; set; }
        [LoadColumn(9)]
        public float proanthocyanins { get; set; }
        [LoadColumn(10)]
        public float color_intensity { get; set; }
        [LoadColumn(11)]
        public float hue { get; set; }
        [LoadColumn(12)]
        public float diluted { get; set; }
        [LoadColumn(13)]
        public float proline { get; set; }
    }
    public class WinePrediction
    {
        [ColumnName("PredictedLabel")]
        public string type { get; set; }
        [ColumnName("Score")]
        public float[] score { get; set; }
    }
}
