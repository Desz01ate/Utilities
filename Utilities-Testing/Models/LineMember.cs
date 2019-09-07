using System;

namespace Utilities.Testing.Models
{
//You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
//[Utilities.Attributes.SQL.Table("[LineMember]")]
public class LineMember
{
    public string Iss { get; set; }
    public string Sub { get; set; }
    public string Aud { get; set; }
    public int Exp { get; set; }
    public int Iat { get; set; }
    public string Amr { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public string Email { get; set; }
}
}
