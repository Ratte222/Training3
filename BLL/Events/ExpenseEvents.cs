using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Events
{
    public class ExpenseEvents
    {
        public event Action<Expense> AddExpense;

        public void AddExpense_Invoke(Expense expense)
        {
            AddExpense?.Invoke(expense);
        }
    }
}
