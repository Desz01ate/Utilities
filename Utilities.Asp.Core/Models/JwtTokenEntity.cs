using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Asp.Core.Models
{
    public class JwtTokenEntity
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
