using AuxiliaryLib.Helpers;
using BLL.DTO.Expense;
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
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;
        public ExpenseController(IExpenseService expenseService, IMapper mapper)
        {
            (_expenseService, _mapper) = (expenseService, mapper);
        }

        [HttpGet("{id}")]
        public IActionResult GetExpese(int id)
        {
            if (id < 1)
                return BadRequest();
            Expense expense = _expenseService.Get(i => { return i.Id == id; });
            var expenseDTO  = _mapper.Map<ExpenseDTO>(expense);
            return Ok(expenseDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses(int? pageLength = null, int? pageNumber = null)
        {
            PageResponse<Expense> pageResponse = new PageResponse<Expense>(pageLength, pageNumber);
            await _expenseService.GetPageResponse(pageResponse);
            var pageResponseDTO = _mapper.Map<PageResponse<ExpenseDTO>>(pageResponse);
            return Ok(pageResponseDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExpenseDTO), 201)]
        public IActionResult Add([FromBody] AddExpenseDTO addExpense)
        {
            Expense expense = _mapper.Map<Expense>(addExpense);
            ExpenseDTO response = _mapper.Map<ExpenseDTO>(_expenseService.Create(expense));
            return Created("Default", response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult Delete(int id)
        {
            if (_expenseService.IsExists(id))
            {
                return NotFound();
            }

            _expenseService.Delete(id);
            return NoContent();
        }
    }
}
