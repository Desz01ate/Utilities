namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("[HeartTraining]")]
    public class HeartTraining
    {
        public double Age { get; set; }
        public double Sex { get; set; }
        public double Cp { get; set; }
        public double ThrestBps { get; set; }
        public double Chol { get; set; }
        public double Fbs { get; set; }
        public double RestEcg { get; set; }
        public double Thalac { get; set; }
        public double Exang { get; set; }
        public double OldPeak { get; set; }
        public double Slope { get; set; }
        public double Ca { get; set; }
        public double Thal { get; set; }
        public string Label { get; set; }
    }
}