using Dapper;
using Data.Dtos.AppUser;
using Data.Dtos.Authenticate;
using Data.Models.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AppUserService : BaseService, IAppUserService
    {
        public AppUserService(IOptions<ConnectionStrings> _connStrings, IHttpContextAccessor contextAccessor) : base(_connStrings, contextAccessor)
        {
        }

        public async Task<AppUserDto> GetUserByEmailAndPassword(AuthenticateRequestParameters requestModel)
        {
            AppUserDto user = new AppUserDto();
            try
            {
                using (var con = new SqlConnection(conStrings.DefaultConnection))
                {
                    string sql = $@"SELECT 
                                    Id,
                                    Email,
                                    FirstName,
                                    LastName
                                    FROM AppUser WITH(NOLOCK)
                                    WHERE
                                    Email = @Email
                                    AND
                                    Password = @Password";


                    user = (await con.QueryAsync<AppUserDto>(sql, new { requestModel.Email, requestModel.Password })).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {

            }

            return user;
        }

    }
}
