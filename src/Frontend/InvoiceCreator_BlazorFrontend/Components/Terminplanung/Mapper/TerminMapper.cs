using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Terminplanung.Mapper
{
    public static class TerminMapper
    {
        public static TerminDto ToCreateDto(Termin termin) => new()
        {
            Start = termin.Start,
            End = termin.End,
            Text = termin.Text,
            KundeId = termin.KundeId,
            UserId = termin.UserId,
            Leistungen = termin.Leistungen != null ? termin.Leistungen : Enumerable.Empty<string>().ToList(),
            GeschätzteDauer = termin.GeschätzteDauer,
            Status = termin.Status,
            BestätigtAm = termin.BestätigtAm
        };

        public static TerminDto ToUpdateDto(Termin termin) => new()
        {
            Id = termin.Id,
            Start = termin.Start,
            End = termin.End,
            Text = termin.Text,
            KundeId = termin.KundeId,
            UserId = termin.UserId,
            Leistungen = termin.Leistungen != null ? termin.Leistungen : Enumerable.Empty<string>().ToList(),
            GeschätzteDauer = termin.GeschätzteDauer,
            Status = termin.Status,
            BestätigtAm = termin.BestätigtAm
        };

        public static Termin FromTerminDto(TerminDto dto) => new()
        {
            Id = dto.Id,
            Start = dto.Start,
            End = dto.End,
            Text = dto.Text,
            KundeId = dto.KundeId,
            UserId = dto.UserId,
            Leistungen = dto.Leistungen != null ? dto.Leistungen : Enumerable.Empty<string>().ToList(),
            GeschätzteDauer = dto.GeschätzteDauer,
            Status = dto.Status,
            BestätigtAm = dto.BestätigtAm
        };
    }
}
