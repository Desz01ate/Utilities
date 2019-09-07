using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[Members]")]
public class Members
{
    //[Utilities.Attributes.SQL.PrimaryKey]
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string LineTokenId { get; set; }
    public string Picture { get; set; }
}
}
