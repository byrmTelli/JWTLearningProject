using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using JWTLearningProject.CORE.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.SERVICE.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<UserAppDTO>> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            var user = new UserApp() { Email = createUserDTO.Email,UserName = createUserDTO.UserName };

            var result = await _userManager.CreateAsync(user,createUserDTO.Password);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();


                return Response<UserAppDTO>.Fail(new ErrorDTO(errors, true), 400);
            }
            return Response<UserAppDTO>.Success(ObjectMapper.Mapper.Map<UserAppDTO>(user), 200);

        }

        public async Task<Response<UserAppDTO>> GetUserByNameASync(string userName)
        {
            var user =await _userManager.FindByNameAsync(userName);

            if(user == null)
            {
                return Response<UserAppDTO>.Fail("Username not found", 404, true);
            }

            return Response<UserAppDTO>.Success(ObjectMapper.Mapper.Map<UserAppDTO>(user), 200);
        }
    }
}
