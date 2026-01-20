using Shared.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator.Tests
{
    public class BankverbindungTests
    {
        private List<ValidationResult> Validate(Bankverbindung bank)
        {
            var context = new ValidationContext(bank);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(bank, context, results, true);
            return results;
        }

        [Fact]
        public void Kontoinhaber_MussVorhandenSein()
        {
            var bank = new Bankverbindung
            {
                Kontoinhaber = "", // ungültig
                IBAN = "DE89370400440532013000",
                BIC = "DEUTDEFFXXX",
                Bankname = "Deutsche Bank"
            };

            var results = Validate(bank);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Kontoinhaber ist erforderlich"));
        }

        [Fact]
        public void IBAN_MussGueltigSein()
        {
            var bank = new Bankverbindung
            {
                Kontoinhaber = "Max Mustermann",
                IBAN = "12345", // ungültig
                BIC = "DEUTDEFFXXX",
                Bankname = "Deutsche Bank"
            };

            var results = Validate(bank);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Ungültige IBAN"));
        }

        [Fact]
        public void BIC_MussGueltigSein()
        {
            var bank = new Bankverbindung
            {
                Kontoinhaber = "Max Mustermann",
                IBAN = "DE89370400440532013000",
                BIC = "123", // ungültig
                Bankname = "Deutsche Bank"
            };

            var results = Validate(bank);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Ungültige BIC"));
        }

        [Fact]
        public void Bankname_MussVorhandenSein()
        {
            var bank = new Bankverbindung
            {
                Kontoinhaber = "Max Mustermann",
                IBAN = "DE89370400440532013000",
                BIC = "DEUTDEFFXXX",
                Bankname = "" // ungültig
            };

            var results = Validate(bank);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Bankname ist erforderlich"));
        }

        [Fact]
        public void Aktiv_DefaultIstTrue()
        {
            var bank = new Bankverbindung
            {
                Kontoinhaber = "Max Mustermann",
                IBAN = "DE89370400440532013000",
                BIC = "DEUTDEFFXXX",
                Bankname = "Deutsche Bank"
            };

            Assert.True(bank.Aktiv);
        }
    }
}
