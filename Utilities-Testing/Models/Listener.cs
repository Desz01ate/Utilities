using System;

namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("[Listener]")]
    public class Listener
    {
        public string Track { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }

        //[Utilities.Attributes.SQL.PrimaryKey]
        public DateTime? CreateDate { get; set; }

        public int? ListeningSeconds { get; set; }
    }
}