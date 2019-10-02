using MachineLearning.Shared.Attributes;
using MachineLearning.Shared.Model;
using MachineLearning.Shared.Model.Interfaces;

namespace MachineLearning.Examples.POCO
{
    public class Iris : IMLConstraint
    {
        public float SepalLength { get; set; }
        public float SepalWidth { get; set; }
        public float PetalLength { get; set; }
        public float PetalWidth { get; set; }
        [LabelColumn]
        public string Label { get; set; }

        public float Actual_float_Result()
        {
            return 0f;
        }

        public string Actual_string_Result()
        {
            return Label;
        }

        public uint Actual_uint_Result()
        {
            return 0u;
        }
    }
    public class IrisClassification : MulticlassClassficationPredictionResult
    {
        //public int TranslateLabelIntoNumber()
        //{
        //    switch (this.PredictedLabel)
        //    {
        //        case "Iris-setosa":
        //            return 1;
        //        case ("Iris-versicolor"):
        //            return 2;
        //        case ("Iris-virginica"):
        //            return 3;
        //    }
        //    return 0;
        //}
        public bool IsCorrectPredict(Iris iris)
        {
            return iris.Label == this.PredictedLabel;
        }
    }
    public class IrisClustering : MachineLearning.Shared.Model.ClusteringPredictionResult
    {

    }
}
