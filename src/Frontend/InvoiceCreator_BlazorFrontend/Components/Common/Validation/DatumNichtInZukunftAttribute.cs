using System.ComponentModel.DataAnnotations;

namespace InvoiceCreator_BlazorFrontend.Components.Common.Validation
{
    public class DatumNichtInZukunftAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Today;
            }
            return false;
        }
    }
}
