namespace IssueProject.Models.User
{
    public class LoginQuery
    {
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}
