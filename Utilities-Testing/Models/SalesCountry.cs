using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[SalesCountry]")]
public class SalesCountry
{
    public double next { get; set; }
    public string country { get; set; }
    public double year { get; set; }
    public double month { get; set; }
    public double max { get; set; }
    public double min { get; set; }
    public double std { get; set; }
    public double count { get; set; }
    public double sales { get; set; }
    public double med { get; set; }
    public double prev { get; set; }
}
}
