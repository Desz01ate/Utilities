namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("[forestfires]")]
    public class forestfires
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public double FFMC { get; set; }
        public double DMC { get; set; }
        public double DC { get; set; }
        public double ISI { get; set; }
        public double temp { get; set; }
        public int RH { get; set; }
        public double wind { get; set; }
        public double rain { get; set; }
        public double area { get; set; }
    }
}