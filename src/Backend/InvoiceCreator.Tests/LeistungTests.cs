using Shared.Domain.Enums;
using Shared.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class LeistungTests
    {
        private List<ValidationResult> Validate(Leistung leistung)
        {
            var context = new ValidationContext(leistung);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(leistung, context, results, true);
            return results;
        }

        [Fact]
        public void Bezeichnung_MussVorhandenSein()
        {
            var leistung = new Leistung
            {
                Bezeichnung = "", // ungültig
                Richtzeit = TimeSpan.FromHours(1),
                Pauschalpreis = 100,
                Pauschalgrenze = TimeSpan.FromMinutes(30),
                PreisPro15Min = 20,
                Steuersatz = Steuersatz.NeunzehnProzent
            };

            var results = Validate(leistung);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Bezeichnung ist erforderlich"));
        }

        [Fact]
        public void Pauschalpreis_MussPositivSein()
        {
            var leistung = new Leistung
            {
                Bezeichnung = "Test",
                Richtzeit = TimeSpan.FromHours(1),
                Pauschalpreis = -5, // ungültig
                Pauschalgrenze = TimeSpan.FromMinutes(30),
                PreisPro15Min = 20,
                Steuersatz = Steuersatz.NeunzehnProzent
            };

            var results = Validate(leistung);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Pauschalpreis muss positiv sein"));
        }

        [Fact]
        public void Beschreibung_DarfMax500ZeichenSein()
        {
            var leistung = new Leistung
            {
                Bezeichnung = "Test",
                Richtzeit = TimeSpan.FromHours(1),
                Pauschalpreis = 100,
                Pauschalgrenze = TimeSpan.FromMinutes(30),
                PreisPro15Min = 20,
                Steuersatz = Steuersatz.NeunzehnProzent,
                Beschreibung = new string('x', 600) // zu lang
            };

            var results = Validate(leistung);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Beschreibung darf maximal 500 Zeichen"));
        }
    }
}
