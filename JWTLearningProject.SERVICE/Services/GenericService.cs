﻿using JWTLearningProject.CORE.Repositories;
using JWTLearningProject.CORE.Services;
using JWTLearningProject.CORE.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JWTLearningProject.SERVICE.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
            await _genericRepository.AddAsync(newEntity);


            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(entity);

            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());


            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if(product == null)
            {
                return Response<TDto>.Fail("id not found",404,true);
            }

            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDTO>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);

            if(isExistEntity==null)
            {
                return Response<NoDataDTO>.Fail("idn ot found",404,true);
            }

            _genericRepository.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDTO>.Success(200);
        }

        public async Task<Response<NoDataDTO>> Update(TDto entity,int id)
        {
            var isExist = await _genericRepository.GetByIdAsync(id);

            if(isExist == null)
            {
                return Response<NoDataDTO>.Fail("id not found", 404, true);
            }

            var updatedEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
            _genericRepository.Update(updatedEntity);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDTO>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);
            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
