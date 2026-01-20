namespace Data.Persistence.Documents
{
    public class UserDocument : BaseDocument
    {
        public string UserName { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Initialen { get; set; }
        public string Email { get; set; }
        public string Rolle { get; set; }
        public DateTime LetzterLogin { get; set; }
    }
}
