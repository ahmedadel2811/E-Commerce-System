using ECommerce.Core.DTOs.category;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<Response<IEnumerable<CategoryWithSubCategoriesDto>>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("main")]
        public async Task<ActionResult<Response<IEnumerable<CategoryDto>>>> GetMainCategories()
        {
            var result = await _categoryService.GetMainCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("GetCategoryById")]
        public async Task<ActionResult<Response<CategoryDetailDto>>> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{parentId}/subcategories")]
        public async Task<ActionResult<Response<IEnumerable<CategoryDto>>>> GetSubCategories(int parentId)
        {
            var result = await _categoryService.GetSubCategoriesAsync(parentId);

            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<CategoryDto>>> CreateCategory(CreateCategoryDto categoryDto)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok( result);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Response<CategoryDto>>> UpdateCategory( UpdateCategoryDto categoryDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(categoryDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<Response<bool>>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}/has-products")]
        public async Task<ActionResult<Response<bool>>> CategoryHasProducts(int id)
        {
            var result = await _categoryService.CategoryHasProductsAsync(id);
            return Ok(result);
        }
    }
}