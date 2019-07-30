using System;

public class ElectionCommittee
{
    public string CommitteeCode { get; set; }
    public string ElectionCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PositionName { get; set; }
    public string AssignSecretKey { get; set; }
    public DateTime? AssignDate { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? Sequence { get; set; }
}

