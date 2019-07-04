using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Attributes.SQL;

namespace MachineLearning.Examples.POCO
{
    [Table("iris")]
    public class Mock
    {
        [Field("SepalLength")]
        public float A { get; set; }
        [Field("SepalWidth")]
        public float B { get; set; }
        [Field("PetalLength")]
        public float C { get; set; }
        [Field("PetalWidth")]
        public float D { get; set; }
        public string Label { get; set; }

    }
}
