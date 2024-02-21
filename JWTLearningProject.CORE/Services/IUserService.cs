using JWTLearningProject.CORE.DTOs;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.CORE.Services
{
    public interface IUserService
    {
        Task<Response<UserAppDTO>> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<Response<UserAppDTO>> GetUserByNameASync(string userName);

    }
}
