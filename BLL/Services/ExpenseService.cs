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
using BLL.Events;

namespace BLL.Services
{
    public class ExpenseService : BaseService<Expense, AppDBContext>, IExpenseService
    {
        private readonly ExpenseEvents _expenseEvents;
        public ExpenseService(AppDBContext appDBContext, ExpenseEvents expenseEvents) : base(appDBContext)
        {
            (_expenseEvents) = (expenseEvents);
        }

        public override Expense Create(Expense model)
        {
            //Expense expense = base.Create(model);
            _expenseEvents.AddExpense_Invoke(model);
            return model;
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
