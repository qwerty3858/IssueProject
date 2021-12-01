namespace IssueProject.Enums.Confirm
{
    public enum ActivityStatuses : byte
    {
        Calisiyor = 0,
        BimOnayBekleme = 1,
        BimOnay = 2,
        DepartmanOnay = 3,
        YazanDepartmanAmirOnay = 4,
        Kilitli = 5,
        RedYapilmayacak = 6,
        Onaylandi = 7
    }

    public enum ConfirmStatuses:byte
    {
        MailGonderilmedi=0,
        MailGonderildiBeklemede=1,
        Onaylandi=2,
        Reddedildi=3
    }
}