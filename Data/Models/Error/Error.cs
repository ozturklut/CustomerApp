using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Error
{
    public class Error : Exception
    {
        public Error(ErrorCode ErrorCode, string Message, string? Key = null)
        {
            this.ErrorCode = ErrorCode;
            this.Message = Message;
            this.Key = Key;
        }

        public ErrorCode ErrorCode { get; set; }
        new public string Message { get; set; }
        public string? Key { get; set; }
    }
}
