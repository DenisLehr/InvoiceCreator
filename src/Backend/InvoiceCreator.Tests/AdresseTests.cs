using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class AdresseTests
    {
        private List<ValidationResult> Validate(Adresse adresse)
        {
            var context = new ValidationContext(adresse);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(adresse, context, results, true);
            return results;
        }

        [Fact]
        public void Strasse_MussVorhandenSein()
        {
            var adresse = new Adresse
            {
                Strasse = "",
                Hausnummer = "12",
                Stadt = "Bochum",
                PLZ = "44787",
                Land = Land.Deutschland
            };

            var results = Validate(adresse);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Straße ist erforderlich"));
        }

        [Fact]
        public void Hausnummer_MussZwischen1Und9999Sein()
        {
            var adresse = new Adresse
            {
                Strasse = "Musterstraße",
                Hausnummer = "99999", // ungültig
                Stadt = "Bochum",
                PLZ = "44787",
                Land = Land.Deutschland
            };

            var results = Validate(adresse);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Hausnummer muss eine Zahl zwischen 1 und 9999 sein"));
        }

        [Fact]
        public void PLZ_MussGenau5ZiffernHaben()
        {
            var adresse = new Adresse
            {
                Strasse = "Musterstraße",
                Hausnummer = "12",
                Stadt = "Bochum",
                PLZ = "1234", // ungültig
                Land = Land.Deutschland
            };

            var results = Validate(adresse);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("PLZ muss genau 5 Ziffern enthalten"));
        }

        [Fact]
        public void Hausnummerzusatz_DarfMax1ZeichenHaben()
        {
            var adresse = new Adresse
            {
                Strasse = "Musterstraße",
                Hausnummer = "12",
                Hausnummerzusatz = "AB", // ungültig
                Stadt = "Bochum",
                PLZ = "44787",
                Land = Land.Deutschland
            };

            var results = Validate(adresse);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Hausnummerzusatz darf maximal 1 Zeichen enthalten"));
        }

        [Fact]
        public void GueltigeAdresse_BestehtValidierung()
        {
            var adresse = new Adresse
            {
                Strasse = "Musterstraße",
                Hausnummer = "12",
                Stadt = "Bochum",
                PLZ = "44787",
                Land = Land.Deutschland
            };

            var results = Validate(adresse);

            Assert.Empty(results); // keine Fehler
        }
    }
}
