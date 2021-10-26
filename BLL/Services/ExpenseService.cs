using BLL.Interfaces;
using BLL.Services.BaseService;
using DAL.EF;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AuxiliaryLib.Helpers;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ExpenseService : BaseService<Expense>, IExpenseService
    {
        public ExpenseService(AppDBContext appDBContext) : base(appDBContext)
        {
            
        }

        public override Expense Get(Func<Expense, bool> func)
        {
            return _context.Expenses.Include(e=>e.Category).FirstOrDefault(func);
        }

        public async Task GetPageResponse(PageResponse<Expense> pageResponse)
        {
            await Paginate(GetAll_Queryable(), pageResponse);
        }
    }
}
