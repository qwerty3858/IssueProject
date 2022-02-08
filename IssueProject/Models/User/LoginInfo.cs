using System;

namespace IssueProject.Models.User
{
    public class LoginInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int Role { get; set; }

        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string TokenType { get; set; } = "";
        public bool IsManager { get; set; } 
        public bool IsVisible { get; set; } 
        public bool IsKeyUser { get; set; } 
        public bool IsCreated { get; set; } 
        public int ValidFor { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
    }
}
