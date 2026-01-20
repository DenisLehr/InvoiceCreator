using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Validation
{
    public class DatumNichtInVergangenheitAttribute:ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date >= DateTime.Today;
            }
            return false;
        }
    }
}
