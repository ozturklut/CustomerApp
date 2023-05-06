using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Extensions
{
    public static class IntExtensions
    {
        public static bool IsNullOrNegative(this int? i)
        {
            if (i == null || i < 0)
            {
                return true;
            }

            return false;
        }
    }
}
