using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(AppDBContext context)
        {
            if(!context.Categories.Any())
            {
                context.Categories.AddRange(new[]
                {
                    new Category()
                    {
                        Name = "Public transport",
                        Aliases = "Bus, trolleybus, tram, route taxi",
                        Is_income = false,
                        Is_base_expense = false,
                        Is_base_income = false
                    },
                    new Category()
                    {
                        Name = "Eat",
                        Aliases = "Product, products, coffe, tea",
                        Is_income = false,
                        Is_base_expense = true,
                        Is_base_income = false
                    },
                    new Category()
                    {
                        Name = "Phone",
                        Aliases = "Operator, kyevstar, vodafone",
                        Is_income = false,
                        Is_base_expense = false,
                        Is_base_income = false
                    },
                    new Category()
                    {
                        Name = "Internet",
                        Aliases = "TRK, fregat",
                        Is_income = false,
                        Is_base_expense = false,
                        Is_base_income = false
                    }
                });
                context.SaveChanges();
            }
            if(!context.Expenses.Any())
            {
                context.AddRange(new[]
                {
                    new Expense()
                    {
                        Amount = 10,
                        CategoryId = 1,
                        Created = DateTime.Now
                    },
                    new Expense()
                    {
                        Amount = 54,
                        CategoryId = 2,
                        Created = DateTime.Now
                    },
                    new Expense()
                    {
                        Amount = 100,
                        CategoryId = 3,
                        Created = DateTime.Now
                    },
                    new Expense()
                    {
                        Amount = 160,
                        CategoryId = 2,
                        Created = DateTime.Now
                    },
                    new Expense()
                    {
                        Amount = 210,
                        CategoryId = 2,
                        Created = DateTime.Now
                    },
                    new Expense()
                    {
                        Amount = 110,
                        CategoryId = 4,
                        Created = DateTime.Now
                    }
                });
                context.SaveChanges();
            }
        }
    }
}
