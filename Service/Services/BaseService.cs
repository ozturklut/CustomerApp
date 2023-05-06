using Data.Models.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class BaseService
    {
        protected readonly ConnectionStrings conStrings;
        public readonly IHttpContextAccessor _contextAccessor;
        public BaseService(IOptions<ConnectionStrings> _connStrings, IHttpContextAccessor contextAccessor)
        {
            this.conStrings = _connStrings.Value;
            _contextAccessor = contextAccessor;
        }
    }
}
