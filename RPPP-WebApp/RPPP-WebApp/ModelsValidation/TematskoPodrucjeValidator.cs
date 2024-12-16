using FluentValidation;
using RPPP_WebApp.Models;

namespace RPPP_WebApp.ModelsValidation
{
    public class TematskoPodrucjeValidator : AbstractValidator<TematskoPodrucje>
    {
        public TematskoPodrucjeValidator() 
        {
            RuleFor(d => d.TematskoPodrucje1)
                .NotEmpty().WithMessage("Tematsko podrucje je obavezno polje.");
        }
    }
}
