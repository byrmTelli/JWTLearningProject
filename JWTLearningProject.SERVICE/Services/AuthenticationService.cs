using JWTLearningProject.CORE.Configuration;
using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using JWTLearningProject.CORE.Repositories;
using JWTLearningProject.CORE.Services;
using JWTLearningProject.CORE.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.SERVICE.Services
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;


        public AuthenticationService
            (
            IOptions<List<Client>> optionClients,
            ITokenService tokenService,
            UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork,
            IGenericRepository<UserRefreshToken> userRefreshTokenService
            )
        {
            _clients = optionClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDTO>> CreateTokenAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null) { throw new ArgumentNullException(nameof(loginDTO)); }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            { return Response<TokenDTO>.Fail("Email or password wrong", 400, true); }

            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            { return Response<TokenDTO>.Fail("Email or password wrong", 400, true); }

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            { await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration }); }

            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();


            return Response<TokenDTO>.Success(token, 200);

        }

        public Response<ClientTokenDTO> CreateTokenByClient(ClientLoginDTO clientLoginDTO)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDTO.ClientId && x.Secret == clientLoginDTO.ClientSecret);

            if(client == null)
            {
                return Response<ClientTokenDTO>.Fail("ClientId or ClientSecret no found", 404, true);
            }

            var token = _tokenService.CreateTokenByClient(client);

            return Response<ClientTokenDTO>.Success(token, 200);
        }

        public async Task<Response<TokenDTO>> CreateTokenByRefreshToken(string refreshToken)
        {
            var refreshTokenExist = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if(refreshTokenExist == null)
            {
                return Response<TokenDTO>.Fail("Refresh token not found", 404,true);
            }

            var user = await _userManager.FindByIdAsync(refreshTokenExist.UserId);

            if(user == null )
            {
                return Response<TokenDTO>.Fail("UserId not found", 404, true);
            }

            var tokenDTO = _tokenService.CreateToken(user);

            refreshTokenExist.Code = tokenDTO.RefreshToken;
            refreshTokenExist.Expiration = tokenDTO.RefreshTokenExpiration;


            await _unitOfWork.CommitAsync();
            return Response<TokenDTO>.Success(tokenDTO, 200);

        }

        public async Task<Response<NoDataDTO>> RevokeRefreshToken(string refreshToken)
        {
            var refreshTokenExist = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if(refreshTokenExist == null)
            {
                return Response<NoDataDTO>.Fail("Refresh token not found", 404,true);
            }

            _userRefreshTokenService.Remove(refreshTokenExist);

            await _unitOfWork.CommitAsync();
            return Response<NoDataDTO>.Success(200);

        }
    }
}
