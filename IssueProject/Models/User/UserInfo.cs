namespace IssueProject.Models.User
{
    public class UserInfo
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public byte RoleId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
       
    }
}