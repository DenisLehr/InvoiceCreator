using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Leistungskatalog.Mapper
{
    public static class LeistungMapper
    {
        public static LeistungDto ToCreateDto(Leistung leistung) => new()
        {
            Code = leistung.Code,
            Bezeichnung = leistung.Bezeichnung,
            Beschreibung = leistung.Beschreibung,
            Richtzeit = leistung.Richtzeit,
            Pauschalpreis = leistung.Pauschalpreis,
            Pauschalgrenze = leistung.Pauschalgrenze,
            PreisPro15Min = leistung.PreisPro15Min,
            IstVorOrt = leistung.IstVorOrt,
            HatZusatzlogik = leistung.HatZusatzlogik,
            Zusatzlogik = leistung.HatZusatzlogik != null ? new ZusatzlogikDto
            {
                Typ = leistung.Zusatzlogik.Typ,
                Grenze = leistung.Zusatzlogik.Grenze,
                PreisProEinheit = leistung.Zusatzlogik.PreisProEinheit
            }: null,
            Steuersatz = leistung.Steuersatz
        };

        public static LeistungDto ToUpdateDto(Leistung leistung) => new()
        {
            Id = leistung.Id,
            Code = leistung.Code,
            Bezeichnung = leistung.Bezeichnung,
            Beschreibung = leistung.Beschreibung,
            Richtzeit = leistung.Richtzeit,
            Pauschalpreis = leistung.Pauschalpreis,
            Pauschalgrenze = leistung.Pauschalgrenze,
            PreisPro15Min = leistung.PreisPro15Min,
            IstVorOrt = leistung.IstVorOrt,
            HatZusatzlogik = leistung.HatZusatzlogik,
            Zusatzlogik = leistung.HatZusatzlogik != null ? new ZusatzlogikDto
            {
                Typ = leistung.Zusatzlogik.Typ,
                Grenze = leistung.Zusatzlogik.Grenze,
                PreisProEinheit = leistung.Zusatzlogik.PreisProEinheit
            }: null,
            Steuersatz = leistung.Steuersatz
        };

        public static Leistung FromLeistungDto(LeistungDto dto) => new()
        {
            Id = dto.Id,
            Code = dto.Code,
            Bezeichnung = dto.Bezeichnung,
            Beschreibung = dto.Beschreibung,
            Richtzeit = dto.Richtzeit,
            Pauschalpreis = dto.Pauschalpreis,
            Pauschalgrenze = dto.Pauschalgrenze,
            PreisPro15Min = dto.PreisPro15Min,
            IstVorOrt = dto.IstVorOrt,
            HatZusatzlogik = dto.HatZusatzlogik,
            Zusatzlogik = dto.Zusatzlogik != null ? new Zusatzlogik
            {
                Typ = dto.Zusatzlogik.Typ,
                Grenze = dto.Zusatzlogik.Grenze,
                PreisProEinheit = dto.Zusatzlogik.PreisProEinheit
            } : null,
            Steuersatz = dto.Steuersatz
        };
    }
}
