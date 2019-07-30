using System;
using System.Data.Common;
using Utilities.Attributes.SQL;
using Utilities.SQL;

public class VoterOTP
{
    [PrimaryKey]
    [IgnoreField(true, true)]
    public int ID { get; set; }
    public string VoterCode { get; set; }
    public DateTime? GeneratedTime { get; set; }
    public DateTime? ExpiredTime { get; set; }
    public string OTP { get; set; }
    public string RefCode { get; set; }
    public bool? Active { get; set; }
    public VoterOTP()
    {

    }
    public VoterOTP(Voter voter, string otp, string refCode)
    {
        VoterCode = voter.VoterCode;
        OTP = otp;
        RefCode = refCode;
        GeneratedTime = DateTime.Now;
        ExpiredTime = DateTime.Now.AddMinutes(5);
        Active = true;
    }
}

