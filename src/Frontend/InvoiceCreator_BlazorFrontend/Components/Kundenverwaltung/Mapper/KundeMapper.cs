using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;
using System.Text;

namespace InvoiceCreator_BlazorFrontend.Components.Kundenverwaltung.Mapper
{
    public static class KundeMapper
    {
        public static KundeDto ToCreateDto(Kunde kunde) => new()
        {
            Vorname = kunde.Vorname,
            Nachname = kunde.Nachname,
            Firmenname = kunde.Firmenname,
            Email = kunde.Email,
            Telefon = kunde.Telefon,
            TelefonMobil = kunde.TelefonMobil,
            Geburtsdatum = kunde.Geburtsdatum,
            Geschlecht = kunde.Geschlecht,
            Adresse = kunde.Adresse == null ? null : new AdresseDto
            {
                Strasse = kunde.Adresse.Strasse,
                Hausnummer = kunde.Adresse.Hausnummer,
                Hausnummerzusatz = kunde.Adresse.Hausnummerzusatz,
                Stadt = kunde.Adresse.Stadt,
                PLZ = kunde.Adresse.PLZ,
                Land = kunde.Adresse.Land
            },
            KundenRabatt = kunde.KundenRabatt,
            NaechsterTermin = kunde.NaechsterTermin == null ? null: new TerminDto
            {
                Id = kunde.NaechsterTermin.Id,
                Start = kunde.NaechsterTermin.Start,
                End = kunde.NaechsterTermin.End,
                Text = kunde.NaechsterTermin.Text,
                KundeId = kunde.NaechsterTermin.KundeId,
                UserId = kunde.NaechsterTermin.UserId,
                Leistungen = kunde.NaechsterTermin.Leistungen,
                GeschätzteDauer = kunde.NaechsterTermin.GeschätzteDauer,
                Status = kunde.NaechsterTermin.Status,
                BestätigtAm = kunde.NaechsterTermin.BestätigtAm
            },
            Termine = (kunde.Termine ?? Enumerable.Empty<Termin>()).Select(t => new TerminDto
            {
                Id = t.Id,
                Start = t.Start,
                End = t.End,
                Text = t.Text,
                KundeId = t.KundeId,
                UserId = t.UserId,
                Leistungen = t.Leistungen,
                GeschätzteDauer = t.GeschätzteDauer,
                Status = t.Status,
                BestätigtAm = t.BestätigtAm
            }).ToList()
        };

        public static KundeDto ToUpdateDto(Kunde kunde) => new()
        {
            Id = kunde.Id!,
            Vorname = kunde.Vorname,
            Nachname = kunde.Nachname,
            Firmenname = kunde.Firmenname,
            Email = kunde.Email,
            Telefon = kunde.Telefon,
            TelefonMobil = kunde.TelefonMobil,
            Geburtsdatum = kunde.Geburtsdatum,
            Geschlecht = kunde.Geschlecht,
            Adresse = kunde.Adresse == null ? null : new AdresseDto
            {
                Strasse = kunde.Adresse.Strasse,
                Hausnummer = kunde.Adresse.Hausnummer,
                Hausnummerzusatz = kunde.Adresse.Hausnummerzusatz,
                Stadt = kunde.Adresse.Stadt,
                PLZ = kunde.Adresse.PLZ,
                Land = kunde.Adresse.Land
            },
            KundenRabatt = kunde.KundenRabatt,
            NaechsterTermin = kunde.NaechsterTermin == null ? null : new TerminDto
            {
                Id = kunde.NaechsterTermin.Id,
                Start = kunde.NaechsterTermin.Start,
                End = kunde.NaechsterTermin.End,
                Text = kunde.NaechsterTermin.Text,
                KundeId = kunde.NaechsterTermin.KundeId,
                UserId = kunde.NaechsterTermin.UserId,
                Leistungen = kunde.NaechsterTermin.Leistungen,
                GeschätzteDauer = kunde.NaechsterTermin.GeschätzteDauer,
                Status = kunde.NaechsterTermin.Status,
                BestätigtAm = kunde.NaechsterTermin.BestätigtAm
            },
            Termine = (kunde.Termine ?? Enumerable.Empty<Termin>()).Select(t => new TerminDto
            {
                Id = t.Id,
                Start = t.Start,
                End = t.End,
                Text = t.Text,
                KundeId = t.KundeId,
                UserId = t.UserId,
                Leistungen = t.Leistungen,
                GeschätzteDauer = t.GeschätzteDauer,
                Status = t.Status,
                BestätigtAm = t.BestätigtAm
            }).ToList()
        };

        public static Kunde FromKundeDto(KundeDto dto) => new()
        {
            Id = dto.Id,
            Vorname = dto.Vorname,
            Nachname = dto.Nachname,
            Firmenname = dto.Firmenname,
            Email = dto.Email,
            Telefon = dto.Telefon,
            TelefonMobil = dto.TelefonMobil,
            Geburtsdatum = dto.Geburtsdatum,
            Geschlecht = dto.Geschlecht,
            Adresse = dto.Adresse == null ? null : new Adresse
            {
                Strasse = dto.Adresse.Strasse,
                Hausnummer = dto.Adresse.Hausnummer,
                Hausnummerzusatz = dto.Adresse.Hausnummerzusatz,
                Stadt = dto.Adresse.Stadt,
                PLZ = dto.Adresse.PLZ,
                Land = dto.Adresse.Land
            },
            KundenRabatt = dto.KundenRabatt,
            NaechsterTermin = dto.NaechsterTermin == null ? null : new Termin
            {
                Id = dto.NaechsterTermin.Id,
                Start = dto.NaechsterTermin.Start,
                End = dto.NaechsterTermin.End,
                Text = dto.NaechsterTermin.Text,
                KundeId = dto.NaechsterTermin.KundeId,
                UserId = dto.NaechsterTermin.UserId,
                Leistungen = dto.NaechsterTermin.Leistungen,
                GeschätzteDauer = dto.NaechsterTermin.GeschätzteDauer,
                Status = dto.NaechsterTermin.Status,
                BestätigtAm = dto.NaechsterTermin.BestätigtAm
            },
            Termine = (dto.Termine ?? Enumerable.Empty<TerminDto>()).Select(t => new Termin
            {
                Id = t.Id,
                Start = t.Start,
                End = t.End,
                Text = t.Text,
                KundeId = t.KundeId,
                UserId = t.UserId,
                Leistungen = t.Leistungen,
                GeschätzteDauer = t.GeschätzteDauer,
                Status = t.Status,
                BestätigtAm = t.BestätigtAm
            }).ToList()
        };
    }
}
