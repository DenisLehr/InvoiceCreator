using QuestPDF.Infrastructure;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.PdfGenerator;

namespace InvoiceCreator.Tests
{
    public class PdfGeneratorServiceTests
    {
        private readonly PdfGeneratorService _service = new();

        private Rechnung CreateTestRechnung()
        {
            return new Rechnung
            {
                Rechnungsnummer = "RE-202511280100-CS",
                Rechnungsdatum = DateTime.UtcNow,
                KundeID = "K001",
                Rechnungsposten = new List<Rechnungsposten>
            {
                new Rechnungsposten
                {
                    LeistungID = "L001",
                    Bezeichnung = "PC-Wartung",
                    Menge = 1,
                    Einzelpreis = 100,
                    Steuersatz = Steuersatz.NeunzehnProzent
                }
            },
                Zahlungsbedingungen = "Bitte überweisen Sie den Rechnungsbetrag innerhalb von 14 Tagen ohne Abzug."
            };
        }

        private Firma CreateTestFirma()
        {
            return new Firma
            {
                Name = "Testfirma GmbH",
                Kontaktperson = "Max Mustermann",
                Adresse = new Adresse { Strasse = "Musterstr.", Hausnummer = "1", PLZ = "12345", Stadt = "Bochum", Land = Land.Deutschland },
                Email = "info@testfirma.de",
                Telefon = "0123456789",
                Bankverbindung = new Bankverbindung { Kontoinhaber = "Testfirma GmbH", IBAN = "DE89370400440532013000", BIC = "DEUTDEFFXXX", Bankname = "Testbank" }
            };
        }

        private Kunde CreateTestKunde()
        {
            return new Kunde
            {
                Vorname = "Anna",
                Nachname = "Schmidt",
                Email = "anna.schmidt@test.de",
                Telefon = "0987654321",
                Geburtsdatum = DateTime.UtcNow.AddYears(-30),
                Geschlecht = Geschlecht.weiblich,
                Adresse = new Adresse { Strasse = "Kundenweg", Hausnummer = "12", PLZ = "54321", Stadt = "Essen", Land = Land.Deutschland }
            };
        }

        [Fact]
        public void GeneriereRechnung_ReturnsPdfBytes()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var rechnung = CreateTestRechnung();
            var firma = CreateTestFirma();
            var kunde = CreateTestKunde();

            
            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Logos", "ciblu_logo_gross.png");
            var logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;

            var pdfBytes = _service.GeneriereRechnung(rechnung, kunde, firma, logoBytes);

            Assert.NotNull(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

    }
}
