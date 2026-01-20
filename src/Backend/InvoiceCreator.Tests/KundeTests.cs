using Shared.Domain.Enums;
using Shared.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class KundeTests
    {
        private List<ValidationResult> Validate(Kunde kunde)
        {
            var context = new ValidationContext(kunde);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(kunde, context, results, true);
            return results;
        }

        [Fact]
        public void Vorname_MussVorhandenSein()
        {
            var kunde = new Kunde
            {
                Vorname = "", // ungültig
                Nachname = "Mustermann",
                Email = "kunde@test.de",
                Telefon = "0123456789",
                Geburtsdatum = DateTime.UtcNow.AddYears(-20),
                Geschlecht = Geschlecht.maennlich
            };

            var results = Validate(kunde);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Vorname ist erforderlich"));
        }

        [Fact]
        public void Email_MussGueltigSein()
        {
            var kunde = new Kunde
            {
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "ungültig@", // ungültig
                Telefon = "0123456789",
                Geburtsdatum = DateTime.UtcNow.AddYears(-20),
                Geschlecht = Geschlecht.maennlich
            };

            var results = Validate(kunde);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("gültige E-Mail-Adresse"));
        }

        [Fact]
        public void Geburtsdatum_DarfNichtInZukunftLiegen()
        {
            var kunde = new Kunde
            {
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "kunde@test.de",
                Telefon = "0123456789",
                Geburtsdatum = DateTime.UtcNow.AddDays(1), // Zukunft
                Geschlecht = Geschlecht.maennlich
            };

            var results = Validate(kunde);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Geburtsdatum darf nicht in der Zukunft liegen"));
        }

        [Fact]
        public void IstB2B_WirdKorrektBerechnet()
        {
            var kunde = new Kunde
            {
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "kunde@test.de",
                Telefon = "0123456789",
                Geburtsdatum = DateTime.UtcNow.AddYears(-20),
                Geschlecht = Geschlecht.maennlich,
                Firmenname = "Testfirma GmbH"
            };

            Assert.True(kunde.IstB2B);

        }
    }
}
