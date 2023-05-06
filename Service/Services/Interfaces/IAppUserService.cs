using Data.Dtos.AppUser;
using Data.Dtos.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IAppUserService
    {
        Task<AppUserDto> GetUserByEmailAndPassword(AuthenticateRequestParameters requestModel);

    }
}
