using System;

public class ElectionConfig
{
    public int ID { get; set; }
    public string ElectionCode { get; set; }
    public string ConfigCode { get; set; }
    public string ConfigName { get; set; }
    public string ConfigValue1 { get; set; }
    public string ConfigValue2 { get; set; }
    public string ConfigValue3 { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

