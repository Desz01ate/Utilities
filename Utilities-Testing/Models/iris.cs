using System;
using Utilities.Interfaces;

namespace Utilities.Testing.Models
{
    public class iris : ICSVReader
    {
        public double? SepalLength { get; set; }
        public double SepalWidth { get; set; }
        public double PetalLength { get; set; }
        public double PetalWidth { get; set; }
        public string Label { get; set; }

        public void ReadFromCSV(string content)
        {
            var contents = content.Split();
            SepalLength = double.Parse(contents[0]);
            SepalWidth = double.Parse(contents[1]);
            PetalLength = double.Parse(contents[2]);
            PetalWidth = double.Parse(contents[3]);
            Label = contents[4];
        }
    }
}
