using Shared.Domain.Enums;
using System.Text.Json.Serialization;

namespace Shared.Dtos
{
    public class AdresseDto
    {
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string? Hausnummerzusatz {  get; set; }
        public string Stadt {  get; set; }
        public string PLZ {  get; set; }
        public Land Land {  get; set; }
    }
}
