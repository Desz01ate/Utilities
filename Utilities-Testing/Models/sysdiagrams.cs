using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[sysdiagrams]")]
public class sysdiagrams
{
    public string name { get; set; }
    //[Utilities.Attributes.SQL.PrimaryKey]
    public int principal_id { get; set; }
    public int diagram_id { get; set; }
    public int? version { get; set; }
    public byte[] definition { get; set; }
}
}
