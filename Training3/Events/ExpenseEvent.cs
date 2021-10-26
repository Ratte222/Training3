using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.Events
{
    public class ExpenseEvent
    {
        public delegate void AddExpenseEvent();
        public event AddExpenseEvent AddExpense;
    }
}
