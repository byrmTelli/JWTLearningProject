using JWTLearningProject.CORE.Configuration;
using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.CORE.Services
{
    public interface ITokenService
    {

        TokenDTO CreateToken(UserApp userApp);
        ClientTokenDTO CreateTokenByClient(Client client);

    }
}
