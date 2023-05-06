using Data.Dtos.Customer;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validations.FluentValidation
{
    public class CustomerUpdateValidator : AbstractValidator<UpdateCustomerRequestParameters>
    {
        public CustomerUpdateValidator()
        {
            RuleFor(x => x.FirstName).Length(2, 50);
            RuleFor(x => x.LastName).Length(2, 50);
            RuleFor(x => x.TCKN).Length(11).Matches("^[0-9]"); ;
            RuleFor(x => x.Birthdate).NotNull();
        }
    }
}
