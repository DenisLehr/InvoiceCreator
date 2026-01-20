using Shared.Domain.Enums;

namespace Shared.Dtos
{
    public class ZusatzlogikDto
    {
        public ZusatzLogikTyp Typ {  get; set; }
        public decimal Grenze { get; set; }
        public decimal PreisProEinheit { get; set; }
    }
}
