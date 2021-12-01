using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(AppDBContext context,  List<Category> categories,
            List<Expense> expenses)
        {
            if(!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
            if(!context.Expenses.Any())
            {
                context.AddRange(expenses);
                context.SaveChanges();
            }
        }        
    }
}
