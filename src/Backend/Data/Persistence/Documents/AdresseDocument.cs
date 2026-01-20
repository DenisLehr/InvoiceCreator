namespace Data.Persistence.Documents
{
    public class AdresseDocument
    {
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string? Hausnummerzusatz { get; set; }
        public string Stadt { get; set; }
        public string PLZ { get; set; }
        public string Land { get; set; }
    }
}
