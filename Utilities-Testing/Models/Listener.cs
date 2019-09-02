using System;

namespace Utilities.Testing.Models
{
public class Listener
{
    public string Track { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? ListeningSeconds { get; set; }
}
}