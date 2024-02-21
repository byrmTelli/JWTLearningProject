using JWTLearningProject.CORE.Configuration;
using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using JWTLearningProject.CORE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace JWTLearningProject.SERVICE.Services
{
    public class TokenService : ITokenService
    {

        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserManager<UserApp> userManager,IOptions<CustomTokenOption> options)
        {
            _tokenOption = options.Value;
            _userManager = userManager;
        }


        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];

            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);


            return Convert.ToBase64String(numberByte);
        }


        private IEnumerable<Claim> GetClaims(UserApp userApp ,List<String> audiences)
        {
            var userList = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud,x)));
            return userList;
        }


        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());

            return claims;

        }
        public TokenDTO CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignInService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
            SigningCredentials signInCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);


            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                //Bu tokeni yayınlayan kim ?
                issuer: _tokenOption.Issuer,
                //Token süresi
                expires: accessTokenExpiration,
                //expires ile not Before arasında belirtilen alanda geçerli olur.
                notBefore: DateTime.Now,
                //
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials:signInCredential
                );

            var handler = new JwtSecurityTokenHandler();


            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDTO() 
            { 
                AccessToken = token, 
                RefreshToken = CreateRefreshToken(), 
                AccessTokenExpiration = accessTokenExpiration, 
                RefreshTokenExpiration =  refreshTokenExpiration
            };



            return tokenDto;
        }

        public ClientTokenDTO CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignInService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
            SigningCredentials signInCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);


            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                //Bu tokeni yayınlayan kim ?
                issuer: _tokenOption.Issuer,
                //Token süresi
                expires: accessTokenExpiration,
                //expires ile not Before arasında belirtilen alanda geçerli olur.
                notBefore: DateTime.Now,
                //
                claims: GetClaimsByClient(client),
                signingCredentials: signInCredential
                );

            var handler = new JwtSecurityTokenHandler();


            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new ClientTokenDTO()
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };



            return tokenDto;
        }
    }
}
