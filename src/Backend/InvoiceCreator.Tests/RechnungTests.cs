using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace InvoiceCreator.Tests
{
    public class RechnungTests
    {
        [Fact]
        public void SummenBerechnung_Korrekt()
        {
            var posten = TestRechnungspostenFactory.CreateList();

            var rechnung = new Rechnung
            {
                Rechnungsposten = posten
            };

            Assert.True(rechnung.PostenNettoSumme > 0);
            Assert.True(rechnung.PostenBruttoSumme > rechnung.PostenNettoSumme);
            Assert.Equal(rechnung.PostenBruttoSumme - rechnung.PostenNettoSumme, rechnung.PostenSteuerSumme);
        }

        [Fact]
        public void RabattUndSkonto_WerdenKorrektBerechnet()
        {
            var posten = TestRechnungspostenFactory.CreateList();

            var rechnung = new Rechnung
            {
                Rechnungsposten = posten,
                Rabatt = 10, // 10 %
                Skonto = 2   // 2 %
            };

            var erwarteterRabatt = Math.Round(rechnung.PostenNettoSumme * 0.10m, 2);
            var erwarteterBruttoNachRabatt = Math.Round(rechnung.PostenBruttoSumme - erwarteterRabatt, 2);
            var erwarteterSkonto = Math.Round(erwarteterBruttoNachRabatt * 0.02m, 2);
            var erwarteterEndbetrag = erwarteterBruttoNachRabatt - erwarteterSkonto;

            Assert.Equal(erwarteterRabatt, rechnung.Rabattbetrag);
            Assert.Equal(erwarteterBruttoNachRabatt, rechnung.BruttobetragNachRabatt);
            Assert.Equal(erwarteterSkonto, rechnung.Skontobetrag);
            Assert.Equal(erwarteterEndbetrag, rechnung.BruttoRechnungsBetrag);
        }

        [Fact]
        public void Standardwerte_WerdenGesetzt()
        {
            var rechnung = new Rechnung
            {
                Rechnungsposten = new List<Rechnungsposten>()
            };

            Assert.Equal(Waehrung.EUR, rechnung.Waehrung);
            Assert.Equal(Zahlungsstatus.Offen, rechnung.Zahlungsstatus);
            Assert.True((rechnung.Faelligkeit - DateTime.UtcNow).TotalDays >= 13.9); // ~14 Tage
        }
    }
}
