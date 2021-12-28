namespace IssueProject.Models.User
{
    public class UserSummary
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsManager { get; set; }
        public bool IsKeyUser { get; set; }
    }
}
