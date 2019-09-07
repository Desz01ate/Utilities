using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[Sales]")]
public class Sales
{
    public double next { get; set; }
    public int productId { get; set; }
    public int year { get; set; }
    public int month { get; set; }
    public double units { get; set; }
    public double avg { get; set; }
    public double count { get; set; }
    public double max { get; set; }
    public double min { get; set; }
    public double prev { get; set; }
}
}
