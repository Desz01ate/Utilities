using Microsoft.ML.Data;

namespace MachineLearning.Examples.POCO
{
    public class Sales
    {
        [LoadColumn(0)]
        public float next { get; set; }
        [LoadColumn(1)]
        public string productId { get; set; }
        [LoadColumn(2)]
        public float year { get; set; }
        [LoadColumn(3)]
        public float month { get; set; }
        [LoadColumn(4)]
        public float units { get; set; }
        [LoadColumn(5)]
        public float avg { get; set; }
        [LoadColumn(6)]
        public float count { get; set; }
        [LoadColumn(7)]
        public float max { get; set; }
        [LoadColumn(8)]
        public float min { get; set; }
        [LoadColumn(9)]
        public float prev { get; set; }
    }
    public class SalesCountry
    {
        public float next { get; set; }
        public string country { get; set; }
        public float year { get; set; }
        public float month { get; set; }
        public float max { get; set; }
        public float min { get; set; }
        public float std { get; set; }
        public float count { get; set; }
        public float sales { get; set; }
        public float med { get; set; }
        public float prev { get; set; }
    }


    public class SalesPrediction
    {
        public float Score;
    }
}
