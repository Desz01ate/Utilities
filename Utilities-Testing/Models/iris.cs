using System;
using Utilities.Attributes.SQL;

namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("[iris]")]
    public class iris
    {
        public double SepalLength { get; set; }
        public double SepalWidth { get; set; }
        [IgnoreField(true, true)]
        public double PetalLength { get; set; }
        [IgnoreField(true, true)]

        public double PetalWidth { get; set; }
        [IgnoreField(true, true)]
        public string Label { get; set; }
    }
}
