using AuxiliaryLib.Helpers;
using BLL.DTO.Category;
using BLL.Interfaces;
using DAL.Entity;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            (_categoryService, _mapper) = (categoryService, mapper);
        }

        [HttpGet("{id}")]
        public IActionResult GetExpese(int id)
        {
            if (id < 1)
                return BadRequest();
            var expense = _mapper.Map<CategoryDTO>(_categoryService.Get(i => { return i.Id == id; }));
            return Ok(expense);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses(int? pageLength = null, int? pageNumber = null)
        {
            PageResponse<Category> pageResponse = new PageResponse<Category>(pageLength, pageNumber);
            await _categoryService.GetPageResponse(pageResponse);
            var pageResponseDTO = _mapper.Map<PageResponse<CategoryDTO>>(pageResponse);
            return Ok(pageResponseDTO);
        }
    }
}
