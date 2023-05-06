using Data.Dtos.AppUser;
using Data.Dtos.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JwtTokenResponse> GenerateToken(AppUserDto user);
        IEnumerable<Claim> GetClaims();
        int GetUserId();
    }
}
