using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Codename { get; set; }
        public string Name { get; set; }
        public bool IsBaseExpense { get; set; }
        public bool IsBaseIncome { get; set; }
        public bool IsIncome { get; set; }
        public string Aliases { get; set; }
    }
}
