using JWTLearningProject.CORE.DTOs;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.CORE.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDTO>> CreateTokenAsync(LoginDTO loginDTO);
        Task<Response<TokenDTO>> CreateTokenByRefreshToken(string refreshToken);
        Task<Response<NoDataDTO>> RevokeRefreshToken(string refreshToken);
        Response<ClientTokenDTO> CreateTokenByClient(ClientLoginDTO clientLoginDTO);
    }
}
