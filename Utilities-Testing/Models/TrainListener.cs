using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[TrainListener]")]
public class TrainListener
{
    public string Track { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
    public string Type { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? ListeningSeconds { get; set; }
}
}
