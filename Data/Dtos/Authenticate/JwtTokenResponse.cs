using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Dtos.Authenticate
{
    public class JwtTokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
