using System;

namespace Utilities.Testing.Models
{
public class Project
{
    public Guid Guid { get; set; }
    public decimal Buyout { get; set; }
    public bool IsOpening { get; set; }
    public Guid? Winner { get; set; }
}
}
