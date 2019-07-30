using System;

public class ElectionSecurityKey
{
    public int ID { get; set; }
    public string SecurityKeyCode { get; set; }
    public string ElectionCode { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

