namespace Utilities.Testing.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("[Member]")]
    public class Member
    {
        public int id { get; set; }

        //[Utilities.Attributes.SQL.PrimaryKey]
        public string username { get; set; }

        public string hash { get; set; }
        public string salt { get; set; }
    }
}