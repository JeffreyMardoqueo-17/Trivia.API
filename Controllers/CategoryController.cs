using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TriviaGame.Api.Services.Interfaces;

namespace TriviaGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;

        public CategoryController(
            ICategoriesService categoriesService,
            IMapper mapper)
        {
            _categoriesService = categoriesService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DTOs.Category.CategoryResponseDTO>>> GetAllCategories()
        {
            var categories = await _categoriesService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound();
            var categoriesDto = _mapper.Map<IEnumerable<DTOs.Category.CategoryResponseDTO>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DTOs.Category.CategoryResponseDTO>> GetCategoryById(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            var categoryDto = _mapper.Map<DTOs.Category.CategoryResponseDTO>(category);
            return Ok(categoryDto);
        }
    }
}