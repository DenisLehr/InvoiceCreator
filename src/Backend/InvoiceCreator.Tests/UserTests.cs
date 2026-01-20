using Shared.Domain.Enums;
using Shared.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class UserTests
    {
        private List<ValidationResult> Validate(User user)
        {
            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(user, context, results, true);
            return results;
        }

        [Fact]
        public void UserName_MussVorhandenSein()
        {
            var user = new User
            {
                UserName = "", // ungültig
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "max@test.de",
                Rolle = Rolle.Admin
            };

            var results = Validate(user);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Benutzername ist erforderlich"));
        }

        [Fact]
        public void Vorname_DarfNurBuchstabenUndBindestricheEnthalten()
        {
            var user = new User
            {
                UserName = "mmuster",
                Vorname = "M@x", // ungültig
                Nachname = "Mustermann",
                Email = "max@test.de",
                Rolle = Rolle.Admin
            };

            var results = Validate(user);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Nur Buchstaben, Leerzeichen und Bindestriche erlaubt"));
        }

        [Fact]
        public void Initialen_WerdenKorrektBerechnet()
        {
            var user = new User
            {
                UserName = "mmuster",
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "max@test.de",
                Rolle = Rolle.Admin
            };

            Assert.Equal("MM", user.Initialen);
        }

        [Fact]
        public void Email_MussGueltigSein()
        {
            var user = new User
            {
                UserName = "mmuster",
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "ungültig@", // ungültig
                Rolle = Rolle.Admin
            };

            var results = Validate(user);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Ungültige E-Mail-Adresse"));
        }
    }
}
