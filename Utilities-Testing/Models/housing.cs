using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[housing]")]
public class housing
{
    public double? CRIM { get; set; }
    public double? ZN { get; set; }
    public double? INDUS { get; set; }
    public double? CHAS { get; set; }
    public double? NOX { get; set; }
    public double? RM { get; set; }
    public double? AGE { get; set; }
    public double? DIS { get; set; }
    public double? RAD { get; set; }
    public double? TAX { get; set; }
    public double? PTRATIO { get; set; }
    public double? B { get; set; }
    public double? LSTAT { get; set; }
    public double? MEDV { get; set; }
}
}
