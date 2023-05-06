using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
        public int Expire { get; set; }
    }
}
