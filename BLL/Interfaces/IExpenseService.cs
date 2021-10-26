using AuxiliaryLib.Helpers;
using BLL.Interfaces.Base;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IExpenseService:IBaseService<Expense>
    {
        public Task GetPageResponse(PageResponse<Expense> pageResponse);
    }
}
