using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Dtos.Customer
{
    public class AddCustomerRequestParameters
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Birthdate { get; set; }
        public string TCKN { get; set; } = string.Empty;
    }
}
