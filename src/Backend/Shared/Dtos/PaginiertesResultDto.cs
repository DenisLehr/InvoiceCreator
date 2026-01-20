namespace Shared.Dtos
{
    public class PaginiertesResultDto<T>
    {
        public PaginiertesResultDto(IEnumerable<T> dtoListe, int gesamtAnzahl, int elementeProSeite, int aktuelleSeite) 
        {
            DtoListe = dtoListe;
            ElementeProSeite = elementeProSeite;
            AktuelleSeite = aktuelleSeite;
            GesamtAnzahl = gesamtAnzahl;
        }

        public PaginiertesResultDto() { }

        public IEnumerable<T> DtoListe { get; set; }
        public int GesamtAnzahl { get; set; }
        public int ElementeProSeite { get; set; }
        public int AktuelleSeite { get; set; }
        
        public int AnzahlSeiten => (int)Math.Ceiling(GesamtAnzahl / (double)ElementeProSeite);

    }
}
