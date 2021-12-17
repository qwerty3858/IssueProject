namespace IssueProject.Common
{
    public enum ActivityStatuses : byte
    {
        Processing = 0,
        ITWaiting = 1,
        DepartmentWaiting = 2,
        ManagerWaiting = 3,
        ManagerCommitted = 4,
        Locked = 5,
        Rejected = 9
    }

    public enum ConfirmStatuses : byte
    {
        MailSendWaiting = 0,
        MailSent = 1,
        Commited = 2,
        Rejected = 3
    }
}