using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shared.Domain.Extensions;
using Shared.Domain.Models;

namespace Shared.PdfGenerator
{
    public class PdfGeneratorService: IPdfGeneratorService
    {
        public byte[] GeneriereRechnung(Rechnung rechnung, Kunde kunde, Firma firma, byte[]? logo = null)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Element(c => ErstelleHeader(c, rechnung, firma, logo));
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingBottom(10);
                    });

                    page.Content().Element(c => ErstelleInhalt(c, kunde, rechnung));

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private void ErstelleHeader(IContainer container, Rechnung r, Firma firma, byte[]? logo)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Rechnung").FontSize(20).SemiBold();
                    });

                    row.ConstantItem(120).Column(c =>
                    {
                        if (logo != null)
                        {
                            c.Item().Image(logo);
                        }
                        else
                        {
                            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Logos", "ciblu_logo_gross.png");
                            c.Item().Image(logoPath);
                        }

                        c.Item().Height(10);
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(firma.Name).FontSize(14).SemiBold();
                            col.Item().Text($"{firma.Adresse.Strasse} {firma.Adresse.Hausnummer} {firma.Adresse.Hausnummerzusatz}");
                            col.Item().Text($"{firma.Adresse.PLZ} {firma.Adresse.Stadt}");
                            col.Item().Text($"Tel: {firma.Telefon}");
                            col.Item().Text($"E-Mail: {firma.Email}");
                            col.Item().PaddingVertical(10);
                            if (!string.IsNullOrEmpty(firma.UStId))
                                col.Item().Text($"USt-IdNr: {firma.UStId}");
                            if (!string.IsNullOrWhiteSpace(firma.HandelsregisterNr) && !string.IsNullOrWhiteSpace(firma.Registergericht))
                                col.Item().Text($"Handelsregister: {firma.Registergericht} {firma.HandelsregisterNr}");
                            if (firma.Rechtsform is not null)
                                col.Item().Text($"Rechtsform: {firma.Rechtsform.GetDisplayName()}");
                            if (firma.Geschaeftsfuehrer is { Count: > 0 })
                                col.Item().Text($"Geschäftsführer: {string.Join(", ", firma.Geschaeftsfuehrer)}");
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignRight().Text($"Rechnungsnummer: {r.Rechnungsnummer}");
                            col.Item().AlignRight().Text($"Rechnungsdatum: {r.Rechnungsdatum:dd.MM.yyyy}");
                            if (r.HatBestellReferenz)
                                col.Item().AlignRight().Text($"Bestellreferenz: {r.Bestellreferenz}");
                        });
                    });
                });
            });
        }

        private void ErstelleInhalt(IContainer container, Kunde kunde, Rechnung r)
        {
            container.Column(col =>
            {
                col.Item().PaddingTop(10);

                if (kunde.IstB2B || !String.IsNullOrEmpty(kunde.Firmenname))
                {
                    col.Item().Text("Firma");
                    col.Item().Text($"{kunde.Firmenname}");
                    col.Item().Text("z.Hd.:");
                }

                col.Item().Text($"{kunde.Vorname} {kunde.Nachname}");
                col.Item().Text($"{kunde.Adresse.Strasse} {kunde.Adresse.Hausnummer} {kunde.Adresse.Hausnummerzusatz}");
                col.Item().Text($"{kunde.Adresse.PLZ} {kunde.Adresse.Stadt}");

                col.Item().PaddingVertical(10);

                col.Item().Text($"Leistungszeitraum: {r.LeistungszeitraumVon:dd.MM.yyyy} – {r.LeistungszeitraumBis:dd.MM.yyyy}");
                col.Item().Text($"Fälligkeit: {r.Faelligkeit:dd.MM.yyyy}");

                col.Item().PaddingVertical(10);

                col.Item().Text($"{r.Zahlungsbedingungen}");

                col.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Element(e => ErstelleRechnungspostenTabelle(e, r));
                col.Item().PaddingTop(10).Element(e => ErstelleSummenBlock(e, r));
            });
        }

        private void ErstelleRechnungspostenTabelle(IContainer container, Rechnung r)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(4); // Beschreibung
                    c.RelativeColumn(1); // Menge
                    c.RelativeColumn(2); // Einheit
                    c.RelativeColumn(2); // Einzelpreis
                    c.RelativeColumn(2); // Gesamtpreis
                });

                table.Header(header =>
                {
                    void HeaderCell(string text) => header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(text).SemiBold();
                    HeaderCell("Leistung");
                    HeaderCell("Menge");
                    HeaderCell("Einheit");
                    HeaderCell("Einzel");
                    HeaderCell("Gesamt");
                });

                int index = 0;
                foreach (var p in r.Rechnungsposten)
                {
                    var bg = index++ % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                    void Cell(string text) => table.Cell().Background(bg).Padding(5).Text(text);

                    Cell(p.Bezeichnung);
                    Cell($"{p.Menge}");
                    Cell($"{p.Einheit.GetDisplayName()}");
                    Cell($"{p.Einzelpreis:N2} {r.Waehrung}");
                    Cell($"{p.GesamtBruttopreis:N2} {r.Waehrung}");
                }
            });
        }

        private void ErstelleSummenBlock(IContainer container, Rechnung r)
        {
            container.AlignRight().Background(Colors.Grey.Lighten4).Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Zwischensumme (netto):");
                    row.ConstantItem(100).AlignRight().Text($"{r.PostenNettoSumme:N2} {r.Waehrung}");
                });

                if (r.Rabatt > 0)
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Rabatt ({r.Rabatt}%):");
                        row.ConstantItem(100).AlignRight().Text($"– {r.Rabattbetrag:N2} {r.Waehrung}");
                    });

                if (r.Skonto > 0)
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Skonto ({r.Skonto}%):");
                        row.ConstantItem(100).AlignRight().Text($"– {r.Skontobetrag:N2} {r.Waehrung}");
                    });

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Umsatzsteuer gesamt:");
                    row.ConstantItem(100).AlignRight().Text($"{r.PostenSteuerSumme:N2} {r.Waehrung}");
                });

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Gesamtbetrag:");
                    row.ConstantItem(100).AlignRight().Text($"{r.BruttoRechnungsBetrag:N2} {r.Waehrung}").SemiBold();
                });

                col.Item().Text($"Zahlungsstatus: {r.Zahlungsstatus}");
            });

        }

    }
}
