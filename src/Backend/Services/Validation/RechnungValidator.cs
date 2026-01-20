using FluentValidation;
using Shared.Dtos;

namespace Services.Validation
{
    public class RechnungValidator: AbstractValidator<RechnungDto>
    {
        public RechnungValidator() 
        { 
            
        }
    }
}
