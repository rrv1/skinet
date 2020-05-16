using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specificaions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductBrand> productBrandRepo;
        private readonly IMapper mapper;
        private readonly IGenericRepository<ProductType> productTypeRepo;
        public ProductsController(IGenericRepository<Product> productRepo,
                                  IGenericRepository<ProductBrand> productBrandRepo,
                                  IGenericRepository<ProductType> productTypeRepo,
                                  IMapper mapper)
        {
            this.productRepo = productRepo;
            this.productBrandRepo = productBrandRepo;
            this.productTypeRepo = productTypeRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string sort, int? brandId, int? typeId)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(sort, brandId, typeId);

            var products = await productRepo.ListAsync(spec);

            return Ok(mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));

            // return products.Select(product => new ProductToReturnDto
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     Price = product.Price,
            //     PictureUrl = product.PictureUrl,
            //     ProductBrand = product.ProductBrand.Name,
            //     ProductType = product.ProductType.Name
            // }).ToList();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await productRepo.GetEntityWithSpec(spec);

            if (product == null) return NotFound(new ApiResponse(404));

            return mapper.Map<Product, ProductToReturnDto>(product);

            // return new ProductToReturnDto
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     Price = product.Price,
            //     PictureUrl = product.PictureUrl,
            //     ProductBrand = product.ProductBrand.Name,
            //     ProductType = product.ProductType.Name
            // };
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync()
        {
            return Ok(await productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypesAsync()
        {
            return Ok(await productTypeRepo.ListAllAsync());
        }


    }
}