namespace Shared.Dtos
{
    public class RechnungPdfResponseDto
    {
        public RechnungDto? Rechnung { get; set; }
        public byte[]? RechnungPdf { get; set; }
    }
}
