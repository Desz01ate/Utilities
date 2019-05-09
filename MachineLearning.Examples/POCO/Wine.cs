using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using Microsoft.ML.Data;
using System;

namespace MachineLearning.Examples.POCO
{
    public class Wine : IConstraint
    {
        public string type { get; set; }
        public float alcohol { get; set; }
        public float malic_acid { get; set; }
        public float ash { get; set; }
        public float alcilinity_of_ash { get; set; }
        public float magnesium { get; set; }
        public float total_phenols { get; set; }
        public float flavanoids { get; set; }
        public float nonflavanoid_phenols { get; set; }
        public float proanthocyanins { get; set; }
        public float color_intensity { get; set; }
        public float hue { get; set; }
        public float diluted { get; set; }
        public float proline { get; set; }

        public float actual_float_result()
        {
            throw new NotImplementedException();
        }

        public string actual_string_result()
        {
            return type;
        }

        public uint actual_uint_result()
        {
            throw new NotImplementedException();
        }
    }
    public class WineClassfication : PredictionMulticlassClassificationModel<Wine>
    {
        //[ColumnName("PredictedLabel")]
        //public string type { get; set; }
        //[ColumnName("Score")]
        //public float[] score { get; set; }
    }
    public class WineClustering : PredictionClusteringModel<Wine>
    {

    }
}
