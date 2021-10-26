using AuxiliaryLib.Helpers;
using BLL.Interfaces;
using BLL.Services.BaseService;
using DAL.EF;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(AppDBContext context) : base(context) { }

        public async Task GetPageResponse(PageResponse<Category> pageResponse)
        {
            await Paginate(GetAll_Queryable(), pageResponse);
        }
    }
}
