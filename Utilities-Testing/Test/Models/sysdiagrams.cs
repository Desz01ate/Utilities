using System;

namespace Test.Models
{
public class sysdiagrams
{
    public string name { get; set; }
    public int principal_id { get; set; }
    public int diagram_id { get; set; }
    public int? version { get; set; }
    public byte[] definition { get; set; }
}
}
