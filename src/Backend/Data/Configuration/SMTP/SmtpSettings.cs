namespace Data.Configuration.SMTP
{
    public class SmtpSettings
    {
        public string DisplayName { get; set; } = string.Empty;
        public string LexwareEmail {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = false;
        public bool UseStartTls { get; set; } = true;
    }
}
