using JWTLearningProject.CORE.DTOs;
using JWTLearningProject.CORE.Models;
using JWTLearningProject.CORE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTLearningProject.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IGenericService<Product, ProductDTO> _productService;


        public ProductController(IGenericService<Product, ProductDTO> productService)
        {
            _productService = productService;
        }



        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO)
        {
            return ActionResultInstance(await _productService.AddAsync(productDTO));
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return ActionResultInstance(await _productService.GetAllAsync());
        }


        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDTO productDTO)
        {
            return ActionResultInstance(await _productService.Update(productDTO,productDTO.Id));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDTO productDTO)
        {
            return ActionResultInstance(await _productService.Update(productDTO, productDTO.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id )
        {
            return ActionResultInstance(await _productService.Remove(id));
        }

    }
}
