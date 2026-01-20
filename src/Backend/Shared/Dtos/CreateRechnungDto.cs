namespace Shared.Dtos
{
    public class CreateRechnungDto
    {
        public string KundeId { get; set; }
        public string UserInitial {  get; set; }
        public List<RechnungspostenDto> Rechnungsposten { get; set; }
    }
}
