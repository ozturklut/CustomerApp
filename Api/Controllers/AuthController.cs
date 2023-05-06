using Data.Dtos.Authenticate;
using Data.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers;
using Service.Services.Interfaces;
using System.Reflection;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAppUserService _appUserService;

        public AuthController(IAuthService authService, IAppUserService appUserService)
        {
            _authService = authService;
            _appUserService = appUserService;
        }

        /// <summary>
        /// Log user in
        /// </summary>
        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ApiBaseResponseModel<AuthenticateResponseDto>))]
        public async Task<IActionResult> SignIn([FromBody] AuthenticateRequestParameters model)
        {
            ApiBaseResponseModel<AuthenticateResponseDto> responseModel = new ApiBaseResponseModel<AuthenticateResponseDto>();
            try
            {
                var user = await _appUserService.GetUserByEmailAndPassword(model);
                (user == null).ThrowIf("Kullanıcı adı veya şifre hatalı");

                var jwt = await _authService.GenerateToken(user!);
                responseModel.Data = new AuthenticateResponseDto() { Token = jwt.Token, ExpireDate = jwt.ExpireDate };
            }
            catch (Exception ex)
            {
                responseModel.setError(ex, MethodBase.GetCurrentMethod());
            }

            return Ok(responseModel);
        }

    }
}
