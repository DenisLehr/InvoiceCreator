using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class FirmaTests
    {
        private List<ValidationResult> Validate(Firma firma)
        {
            var context = new ValidationContext(firma);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(firma, context, results, true);
            return results;
        }

        [Fact]
        public void Name_MussVorhandenSein()
        {
            var firma = new Firma
            {
                Name = "", // ungültig
                Kontaktperson = "Max Mustermann",
                Adresse = new Adresse { Strasse = "Musterstr.", Hausnummer = "1", PLZ = "12345", Stadt = "Bochum" },
                Email = "info@test.de",
                Telefon = "0123456789",
                Bankverbindung = new Bankverbindung { IBAN = "DE12345678901234567890", BIC = "TESTDEFFXXX" }
            };

            var results = Validate(firma);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Der Firmenname ist erforderlich"));
        }

        [Fact]
        public void Email_MussGueltigSein()
        {
            var firma = new Firma
            {
                Name = "Testfirma",
                Kontaktperson = "Max Mustermann",
                Adresse = new Adresse { Strasse = "Musterstr.", Hausnummer = "1", PLZ = "12345", Stadt = "Bochum" },
                Email = "ungültig@", // ungültig
                Telefon = "0123456789",
                Bankverbindung = new Bankverbindung { IBAN = "DE12345678901234567890", BIC = "TESTDEFFXXX" }
            };

            var results = Validate(firma);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Ungültige E-Mail-Adresse"));
        }

        [Fact]
        public void UStId_MussFormat_DE123456789_Haben()
        {
            var firma = new Firma
            {
                Name = "Testfirma",
                Kontaktperson = "Max Mustermann",
                Adresse = new Adresse { Strasse = "Musterstr.", Hausnummer = "1", PLZ = "12345", Stadt = "Bochum" },
                Email = "info@test.de",
                Telefon = "0123456789",
                Bankverbindung = new Bankverbindung { IBAN = "DE12345678901234567890", BIC = "TESTDEFFXXX" },
                UStId = "DE12345" // ungültig
            };

            var results = Validate(firma);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Die USt-Id muss im Format DE123456789 sein"));
        }

        [Fact]
        public void HandelsregisterNr_MussFormat_HRB_Haben()
        {
            var firma = new Firma
            {
                Name = "Testfirma",
                Kontaktperson = "Max Mustermann",
                Adresse = new Adresse { Strasse = "Musterstr.", Hausnummer = "1", PLZ = "12345", Stadt = "Bochum" },
                Email = "info@test.de",
                Telefon = "0123456789",
                Bankverbindung = new Bankverbindung { IBAN = "DE12345678901234567890", BIC = "TESTDEFFXXX" },
                HandelsregisterNr = "XYZ123" // ungültig
            };

            var results = Validate(firma);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Handelsregister‑Nummer muss im Format HRB 12345"));
        }
    }
}
