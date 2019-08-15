using System;

namespace Utilities.Testing.Models
{
    public class iris
    {
        public double SepalLength { get; set; }
        public double SepalWidth { get; set; }
        public double PetalLength { get; set; }
        public double PetalWidth { get; set; }
        public string Label { get; set; }
        public override string ToString()
        {
            return $"S.Length {SepalLength}, S.Width {SepalWidth}, P.Length {PetalLength}, P.Width {PetalWidth}";
        }
    }
}
