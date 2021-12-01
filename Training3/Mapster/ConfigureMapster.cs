using AuxiliaryLib.Helpers;
using BLL.DTO.Category;
using BLL.DTO.Expense;
using BLL.DTO.Notification;
using DAL.Entity;
using DAL_NS.Entity;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.Mapster
{
    //https://github-wiki-see.page/m/MapsterMapper/Mapster/wiki/Custom-mapping
    public static class ConfigureMapster
    {
        public static void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Expense, ExpenseDTO>();
            config.NewConfig<ExpenseDTO, Expense>();
            config.NewConfig<AddExpenseDTO, Expense>();
            config.NewConfig<PageResponse<Expense>, PageResponse<ExpenseDTO>>();

            config.NewConfig<Category, CategoryDTO>()
                .Map(dest => dest.IsBaseExpense, scr => scr.Is_base_expense)
                .Map(dest => dest.IsBaseIncome, scr => scr.Is_base_income)
                .Map(dest => dest.IsIncome, scr => scr.Is_income);

            config.NewConfig<CategoryDTO, Category>()
                .Map(dest => dest.Is_base_expense, scr => scr.IsBaseExpense)
                .Map(dest => dest.Is_base_income, scr => scr.IsBaseIncome)
                .Map(dest => dest.Is_income, scr => scr.IsIncome);
            config.NewConfig<PageResponse<Category>, PageResponse<CategoryDTO>>();

            config.NewConfig<CreateNotificationDTO, Notification>()
                .Map(dest => dest.DateTimeCreate, scr => DateTime.UtcNow);
        }
    }
}
