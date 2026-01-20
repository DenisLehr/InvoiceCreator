using Services.Fakturierung;
using Shared.Domain.Models;

namespace InvoiceCreator.Tests
{
    public class FakturierungServiceTests
    {
        private readonly FakturierungService _service = new();

        [Fact]
        public void BerechnePreis_DauerUnterPauschale_ReturnsPauschalpreis()
        {
            var leistung = new Leistung
            {
                Pauschalpreis = 100,
                Pauschalgrenze = TimeSpan.FromHours(1),
                PreisPro15Min = 20,
                HatZusatzlogik = false
            };

            var result = InvokePrivateBerechnePreis(leistung, TimeSpan.FromMinutes(30), 0);

            Assert.Equal(100m, result);
        }

        [Theory]
        [InlineData(30, 100)]   // 30 Minuten → Pauschalpreis
        [InlineData(90, 140)]   // 90 Minuten → Pauschalpreis + 2 mal Zuschlag
        public void BerechnePreis_VerschiedeneDauern_ReturnsExpected(decimal minuten, decimal expected)
        {
            var leistung = new Leistung
            {
                Pauschalpreis = 100,
                Pauschalgrenze = TimeSpan.FromMinutes(60),
                PreisPro15Min = 20,
                HatZusatzlogik = false
            };

            var result = InvokePrivateBerechnePreis(leistung, TimeSpan.FromMinutes((double)minuten), 0);

            Assert.Equal(expected, result);
        }

        // Hilfsmethode für private Methoden
        private decimal InvokePrivateBerechnePreis(Leistung l, TimeSpan d, decimal m)
        {
            var method = typeof(FakturierungService).GetMethod("BerechneRechnungspostenPreis",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return (decimal)method.Invoke(_service, new object[] { l, d, m });
        }
    }
}
