namespace Data.Persistence.Documents
{
    public class BankverbindungDocument
    {
        public string Kontoinhaber { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string Bankname { get; set; }
        public bool Aktiv { get; set; }
    }
}
