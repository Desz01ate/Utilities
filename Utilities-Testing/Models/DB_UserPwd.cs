using System;

public class DB_UserPwd
{
    public int ID { get; set; }
    public string UserID { get; set; }
    public string Pwd { get; set; }
    public bool? Active { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}

