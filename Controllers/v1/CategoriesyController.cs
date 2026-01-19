using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController(ICategoryRepository categoryRepository) : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("This endpoint is deprecated. Use GetCategoriesById in v2.")]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = categories.Adapt<List<CategoryDto>>();
            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        // [ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategory(int id)
        {
            Console.WriteLine($"GetCategory called with id: {id} a las {DateTime.Now}");
            var category = _categoryRepository.GetCategory(id);
            Console.WriteLine("Category retrieved: " + (category != null ? category.Name : "null"));
            if (category == null) return NotFound($"Category with Id: {id} was not found.");
            var categoryDto = category.Adapt<CategoryDto>();
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest(ModelState);

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists.");
                return BadRequest(ModelState);
            }

            var category = createCategoryDto.Adapt<Category>();

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong while saving the category {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with Id: {id} was not found.");
            }

            if (updateCategoryDto == null) return BadRequest(ModelState);

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists.");
                return BadRequest(ModelState);
            }

            var category = updateCategoryDto.Adapt<Category>();
            category.Id = id;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong while updating the category {category.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);

            if (category == null) return NotFound($"Category with Id: {id} was not found.");

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong while deleting the category {category!.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
