using ECommerce.Core.DTOs.category;
using ECommerce.Core.DTOs.product;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{

    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        //[Authorize(Policy = "CanViewPolicy")]

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response<IEnumerable<ProductDto>>>> GetProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("GetActiveProducts")]
        public async Task<ActionResult<Response<IEnumerable<ProductDto>>>> GetActiveProducts()
        {
            var result = await _productService.GetActiveProductsAsync();
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<Response<ProductDetailDto>>> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<ProductDto>>> CreateProduct(CreateProductDto productDto)
        {
            var result = await _productService.CreateProductAsync(productDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetProduct), new { id = result.Data.Id }, result);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Response<ProductDto>>> UpdateProduct(int id, UpdateProductDto productDto)
        {
            var result = await _productService.UpdateProductAsync(id, productDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<Response<bool>>> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetProductsByVendor")]
        public async Task<ActionResult<Response<IEnumerable<ProductDto>>>> GetProductsByVendor(int vendorId)
        {
            var result = await _productService.GetProductsByVendorAsync(vendorId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPatch("UpdateStock")]
        public async Task<ActionResult<Response<bool>>> UpdateStock( int id, [FromBody] int quantity)
        {
            var result = await _productService.UpdateProductStockAsync(id, quantity);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("{productId}/categories")]
        public async Task<ActionResult<Response<bool>>> AddCategoriesToProduct(int productId, [FromBody] List<int> categoryIds)
        {
            var result = await _productService.AddCategoriesToProductAsync(productId, categoryIds);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{productId}/categories")]
        public async Task<ActionResult<Response<bool>>> UpdateProductCategories(int productId, [FromBody] List<int> categoryIds)
        {
            var result = await _productService.UpdateProductCategoriesAsync(productId, categoryIds);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{productId}/categories")]
        public async Task<ActionResult<Response<List<CategoryDto>>>> GetProductCategories(int productId)
        {
            var result = await _productService.GetProductCategoriesAsync(productId);
            return Ok(result);
        }


        [HttpGet("GetProductsByCategory")]
        public async Task<ActionResult<Response<IEnumerable<ProductDto>>>>GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);

        }
    }



}