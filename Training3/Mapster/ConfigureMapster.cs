using AuxiliaryLib.Helpers;
using BLL.DTO.Expense;
using DAL.Entity;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.Mapster
{
    public static class ConfigureMapster
    {
        public static void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Expense, ExpenseDTO>();
            config.NewConfig<ExpenseDTO, Expense>();
            config.NewConfig<AddExpenseDTO, Expense>();
            config.NewConfig<PageResponse<Expense>, PageResponse<ExpenseDTO>>();
        }
    }
}
