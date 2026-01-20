namespace Shared.Dtos
{
    public class BankverbindungDto
    {
        public string Kontoinhaber { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string Bankname { get; set; }
    }
}