using Data.Dtos.AppUser;
using Data.Dtos.Authenticate;
using Data.Models.Connections;
using Data.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthService(IOptions<ConnectionStrings> _connStrings, IHttpContextAccessor contextAccessor, IOptions<JwtSettings> jwtSettings) : base(_connStrings, contextAccessor)
        {
            _jwtSettings = jwtSettings.Value;
            _contextAccessor = contextAccessor;
        }

        public async Task<JwtTokenResponse> GenerateToken(AppUserDto user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("UserId" , user.Id.ToString()),
                new Claim("FirstName" , user.FirstName.ToString()),
                new Claim("LastName" , user.LastName.ToString()),
                new Claim("Email" , user.Email.ToString()),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var expireDate = DateTime.Now.AddDays(_jwtSettings.Expire);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                signingCredentials: creds,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expireDate
                );

            return new JwtTokenResponse() { Token = new JwtSecurityTokenHandler().WriteToken(token) ,ExpireDate = expireDate };
        }

        public IEnumerable<Claim> GetClaims()
        {
            var identity = _contextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = new List<Claim>();
            if (identity != null)
            {
                claims = identity.Claims;
            }

            return claims;
        }

        public int GetUserId()
        {
            var claims = GetClaims();
            if (!claims.IsNullOrEmpty())
                return int.Parse(claims.FirstOrDefault(x => x.Type == "UserId")!.Value);

            return 0;
        }
    }
}
