using Shared.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.ValueObjects
{
    public class Zusatzlogik
    {
        [Required(ErrorMessage = "Der Zusatzlogik-Typ ist erforderlich.")]
        [EnumDataType(typeof(ZusatzLogikTyp))]
        public ZusatzLogikTyp Typ { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Die Grenze muss positiv sein.")]
        public decimal Grenze { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Der Preis pro Einheit muss positiv sein.")]
        public decimal PreisProEinheit { get; set; }
    }
}
