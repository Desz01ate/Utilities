using System;

public class ElectionCandidate
{
    public string CandidateCode { get; set; }
    public string ElectionCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PositionName { get; set; }
    public int? BallotNumber { get; set; }
    public string Biography { get; set; }
    public string BiographyURL { get; set; }
    public string Vision { get; set; }
    public string VisionURL { get; set; }
    public string PictureURL { get; set; }
    public int? VoteCount { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string Remark { get; set; }
}

