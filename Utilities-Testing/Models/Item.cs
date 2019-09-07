using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[Item]")]
public class Item
{
    //[Utilities.Attributes.SQL.PrimaryKey]
    public string Id { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
}
}
