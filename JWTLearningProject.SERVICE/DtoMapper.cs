using AutoMapper;
using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.SERVICE
{
    internal class DtoMapper:Profile
    {

        public DtoMapper()
        {
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<UserAppDTO, UserApp>().ReverseMap();
        }

    }
}
