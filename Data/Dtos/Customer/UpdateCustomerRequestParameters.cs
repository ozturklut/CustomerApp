using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Dtos.Customer
{
    public class UpdateCustomerRequestParameters
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCKN { get; set; }
        public string Birthdate { get; set; }
    }
}
