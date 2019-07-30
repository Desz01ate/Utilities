using System;

public class DB_User
{
    public string UserID { get; set; }
    public string UserNo { get; set; }
    public string UserPeopleID { get; set; }
    public string Name { get; set; }
    public string BoardType { get; set; }
    public string PositionCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Address { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string SubDistrict { get; set; }
    public string Zipcode { get; set; }
    public string TelNo { get; set; }
    public string FaxNo { get; set; }
    public string Email { get; set; }
    public string Education { get; set; }
    public string Specialize { get; set; }
    public string TeamType { get; set; }
    public string TeamCode { get; set; }
    public string DepartmentCode { get; set; }
    public string DivisionCode { get; set; }
    public string SupUserID { get; set; }
    public string Status { get; set; }
    public bool? Active { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string LoginType { get; set; }
    public int? FailTimes { get; set; }
    public bool? IsLock { get; set; }
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string RejectReason { get; set; }
    public int? OfficerID { get; set; }
}

