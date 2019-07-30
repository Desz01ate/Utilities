using System;

public class ElectionVoter
{
    public string ElectionCode { get; set; }
    public string VoterCode { get; set; }
    public int? TotalVote { get; set; }
    public int? BalanceVote { get; set; }
    public int? NotVote { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

