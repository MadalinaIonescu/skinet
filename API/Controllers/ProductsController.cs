using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork,
         IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //[Cached(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams); 

            var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            var data = _mapper
                .Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,
             productParams.PageSize, totalItems, data));
        }

        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

            if(product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return _mapper.Map<Product, ProductToReturnDto> (product);
        }

        [Cached(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());
        }

        [Cached(600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> CreateProduct(ProductCreateDto productCreateDto)
        {
            Product product = _mapper.Map<ProductCreateDto, Product>(productCreateDto);
            product.PictureUrl = "images/products/placeholder.png";

            _unitOfWork.Repository<Product>().Add(product);
            var result = await _unitOfWork.Complete();

            if(result <= 0)
            {
                return BadRequest( new ApiResponse(400, "Problem Creating product"));
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, ProductCreateDto productUpdateDto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            productUpdateDto.PictureUrl = product.PictureUrl;

            _mapper.Map(productUpdateDto, product);

           _unitOfWork.Repository<Product>().Update(product);

           var result = await _unitOfWork.Complete();

           if(result <=0)
           {
               return BadRequest(new ApiResponse(400, "Problem updating product"));
           }
           return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            _unitOfWork.Repository<Product>().Delete(product);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return BadRequest(new ApiResponse(400, "Problem updating product"));
            }
            return Ok();
        }
    }
}