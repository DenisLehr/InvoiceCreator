using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Firmendaten.Mapper
{
    public static class FirmaMapper
    {
        public static FirmaDto ToCreateDto(Firma firma) => new()
        {
            Name = firma.Name,
            Kontaktperson = firma.Kontaktperson,
            Geschaeftsfuehrer = firma.Geschaeftsfuehrer,
            Adresse = new AdresseDto { 
                Strasse = firma.Adresse.Strasse,
                Hausnummer = firma.Adresse.Hausnummer,
                Hausnummerzusatz = firma.Adresse.Hausnummerzusatz,
                PLZ = firma.Adresse.PLZ,
                Stadt = firma.Adresse.Stadt,
                Land = firma.Adresse.Land
            },
            Email = firma.Email,
            Telefon = firma.Telefon,
            UStId = firma.UStId,
            Bankverbindung = new BankverbindungDto
            {
                Kontoinhaber = firma.Bankverbindung.Kontoinhaber,
                IBAN = firma.Bankverbindung.IBAN,
                BIC = firma.Bankverbindung.BIC,
                Bankname = firma.Bankverbindung.Bankname
            },
            HandelsregisterNr = firma.HandelsregisterNr,
            Registergericht = firma.Registergericht,
            Rechtsform = firma.Rechtsform
        };

        public static FirmaDto ToUpdateDto(Firma firma) => new()
        {
            Id = firma.Id,
            Name = firma.Name,
            Kontaktperson = firma.Kontaktperson,
            Geschaeftsfuehrer = firma.Geschaeftsfuehrer,
            Adresse = new AdresseDto
            {
                Strasse = firma.Adresse.Strasse,
                Hausnummer = firma.Adresse.Hausnummer,
                Hausnummerzusatz = firma.Adresse.Hausnummerzusatz,
                PLZ = firma.Adresse.PLZ,
                Stadt = firma.Adresse.Stadt,
                Land = firma.Adresse.Land
            },
            Email = firma.Email,
            Telefon = firma.Telefon,
            UStId = firma.UStId,
            Bankverbindung = new BankverbindungDto
            {
                Kontoinhaber = firma.Bankverbindung.Kontoinhaber,
                IBAN = firma.Bankverbindung.IBAN,
                BIC = firma.Bankverbindung.BIC,
                Bankname = firma.Bankverbindung.Bankname
            },
            HandelsregisterNr = firma.HandelsregisterNr,
            Registergericht = firma.Registergericht,
            Rechtsform = firma.Rechtsform
        };

        public static Firma FromFirmaDto(FirmaDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Kontaktperson = dto.Kontaktperson,
            Geschaeftsfuehrer = dto.Geschaeftsfuehrer,
            Email = dto.Email,
            Telefon = dto.Telefon,
            UStId = dto.UStId,
            Adresse = new Adresse
            {
                Strasse = dto.Adresse.Strasse,
                Hausnummer = dto.Adresse.Hausnummer,
                Hausnummerzusatz = dto.Adresse.Hausnummerzusatz,
                PLZ = dto.Adresse.PLZ,
                Stadt = dto.Adresse.Stadt,
                Land = dto.Adresse.Land
            },
            Bankverbindung = new Bankverbindung
            {
                Kontoinhaber = dto.Bankverbindung.Kontoinhaber,
                IBAN = dto.Bankverbindung.IBAN,
                BIC = dto.Bankverbindung.BIC,
                Bankname = dto.Bankverbindung.Bankname
            },
            HandelsregisterNr = dto.HandelsregisterNr,
            Registergericht = dto.Registergericht,
            Rechtsform = dto.Rechtsform
        };
    }
}
