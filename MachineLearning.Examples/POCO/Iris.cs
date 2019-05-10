using MachineLearning.Examples.Classes;
using MachineLearning.Examples.Interfaces;
using MachineLearning.Shared.Attributes;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.POCO
{
    public class Iris : IConstraint
    {
        public float SepalLength { get; set; }
        public float SepalWidth { get; set; }
        public float PetalLength { get; set; }
        public float PetalWidth { get; set; }
        [LabelColumn]
        public string Label { get; set; }
        public float actual_float_result()
        {
            throw new NotImplementedException();
        }

        public string actual_string_result()
        {
            return Label;
        }

        public uint actual_uint_result()
        {
            throw new NotImplementedException();
        }
    }
    public class IrisClassification : PredictionMulticlassClassificationModel<Iris>
    {
        public int TranslateLabelIntoNumber()
        {
            switch (this.Predicted_result)
            {
                case "Iris-setosa":
                    return 1;
                case ("Iris-versicolor"):
                    return 2;
                case ("Iris-virginica"):
                    return 3;
            }
            return 0;
        }
    }
    public class IrisClustering : PredictionClusteringModel<Iris>
    {

    }
}
