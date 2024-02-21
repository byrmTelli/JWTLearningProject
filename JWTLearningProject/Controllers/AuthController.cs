using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTLearningProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        //api/auth/createtoken
        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDTO loginDTO)
        {
            var result = await _authenticationService.CreateTokenAsync(loginDTO);

            return ActionResultInstance(result);

        }


        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDTO clientLoginDTO)
        {
            var result =  _authenticationService.CreateTokenByClient(clientLoginDTO);

            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var result =await  _authenticationService.RevokeRefreshToken(refreshTokenDTO.Token);

            return ActionResultInstance(result);

        }


        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDTO.Token);

            return ActionResultInstance(result);
        }
    }
}
