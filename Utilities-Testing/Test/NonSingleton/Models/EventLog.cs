using System;

namespace Utilities.Testing2.Models
{
public class EventLog
{
    public int log_id { get; set; }
    public Guid user_id { get; set; }
    public string reason { get; set; }
    public DateTime date { get; set; }
}
}
