using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[Project]")]
public class Project
{
    public Guid Guid { get; set; }
    public decimal Buyout { get; set; }
    public bool IsOpening { get; set; }
    public Guid? Winner { get; set; }
}
}
