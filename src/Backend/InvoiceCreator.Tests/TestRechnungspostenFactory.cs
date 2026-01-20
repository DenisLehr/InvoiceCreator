using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace InvoiceCreator.Tests
{
    public static class TestRechnungspostenFactory
    {
        public static Rechnungsposten Create(
        string bezeichnung,
        decimal einzelpreis,
        int menge = 1,
        Steuersatz steuersatz = Steuersatz.NeunzehnProzent,
        string beschreibung = "")
        {
            return new Rechnungsposten
            {
                LeistungID = Guid.NewGuid().ToString(),
                Bezeichnung = bezeichnung,
                Beschreibung = beschreibung,
                Einzelpreis = einzelpreis,
                Menge = menge,
                Steuersatz = steuersatz,
                Position = 1
            };
        }

        public static List<Rechnungsposten> CreateList()
        {
            return new List<Rechnungsposten>
        {
            Create("PC-Wartung", 70, 1),
            Create("Windows-Neuinstallation", 219, 1),
            Create("Datenumzug", 99, 1),
            Create("Drucker einrichten", 79, 1),
            Create("FritzBox einrichten", 129, 1)
        };
        }
    }
}
