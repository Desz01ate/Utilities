using System;

public class Election
{
    public string ElectionCode { get; set; }
    public string DepartmentCode { get; set; }
    public string ElectionName { get; set; }
    public string ElectionShortName { get; set; }
    public DateTime? StartVoteDate { get; set; }
    public DateTime? EndVoteDate { get; set; }
    public string StartSystemStatus { get; set; }
    public DateTime? StartSystemDate { get; set; }
    public string EndSystemStatus { get; set; }
    public DateTime? EndSystemDate { get; set; }
    public string ComputeVoteStatus { get; set; }
    public DateTime? ComputeVoteStartDate { get; set; }
    public DateTime? ComputeVoteEndDate { get; set; }
    public int? TotalVote { get; set; }
    public int? TotalNotVote { get; set; }
    public int? TotalVoteNo { get; set; }
    public int? TotalVoteSuccess { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool? Active { get; set; }
    public bool IsAvailableVoteTime()
    {
        return StartSystemDate < DateTime.Now && DateTime.Now < EndSystemDate;
    }
}

