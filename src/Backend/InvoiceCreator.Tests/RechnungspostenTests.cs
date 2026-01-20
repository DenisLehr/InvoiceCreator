using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class RechnungspostenTests
    {
        private List<ValidationResult> Validate(Rechnungsposten posten)
        {
            var context = new ValidationContext(posten);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(posten, context, results, true);
            return results;
        }

        [Fact]
        public void Nettopreis_WirdKorrektBerechnet()
        {
            var posten = new Rechnungsposten
            {
                LeistungID = "L001",
                Bezeichnung = "PC-Wartung",
                Menge = 2,
                Einzelpreis = 50,
                Steuersatz = Steuersatz.NeunzehnProzent,
                Rabatt = 10 // 10 %
            };

            Assert.Equal(90m, posten.GesamtNettopreis); // 2*50 = 100 - 10% = 90
        }

        [Fact]
        public void Steuerbetrag_WirdKorrektBerechnet()
        {
            var posten = new Rechnungsposten
            {
                LeistungID = "L002",
                Bezeichnung = "Windows-Installation",
                Menge = 1,
                Einzelpreis = 100,
                Steuersatz = Steuersatz.NeunzehnProzent
            };

            Assert.Equal(19m, posten.Steuerbetrag); // 100 * 19% = 19
        }

        [Fact]
        public void Brutto_WirdKorrektBerechnet()
        {
            var posten = new Rechnungsposten
            {
                LeistungID = "L003",
                Bezeichnung = "Datenumzug",
                Menge = 1,
                Einzelpreis = 200,
                Steuersatz = Steuersatz.NeunzehnProzent
            };

            Assert.Equal(238m, posten.GesamtBruttopreis); // 200 + 38 = 238
        }

        [Fact]
        public void Rabatt_MussZwischen0Und100Liegen()
        {
            var posten = new Rechnungsposten
            {
                LeistungID = "L004",
                Bezeichnung = "Drucker einrichten",
                Menge = 1,
                Einzelpreis = 100,
                Steuersatz = Steuersatz.NeunzehnProzent,
                Rabatt = 150 // ungültig
            };

            var results = Validate(posten);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Rabatt muss zwischen 0 und 100 liegen"));
        }
    }
}
