using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[wine]")]
public class wine
{
    public string type { get; set; }
    public double alcohol { get; set; }
    public double malic_acid { get; set; }
    public double ash { get; set; }
    public double alcilinity_of_ash { get; set; }
    public int magnesium { get; set; }
    public double total_phenols { get; set; }
    public double flavanoids { get; set; }
    public double nonflavanoid_phenols { get; set; }
    public double proanthocyanins { get; set; }
    public double color_intensity { get; set; }
    public double hue { get; set; }
    public double diluted { get; set; }
    public int proline { get; set; }
}
}
