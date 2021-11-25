using System;

namespace IssueProject.Models.User
{
    public class LoginInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public short Role { get; set; }

        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string TokenType { get; set; } = "";

        public int ValidFor { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
    }
}
