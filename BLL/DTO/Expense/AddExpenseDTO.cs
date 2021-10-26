using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Expense
{
    public class AddExpenseDTO
    {
        public int Amount { get; set; }
        public DateTime Created { get; set; }
        public int CategoryId { get; set; }
        //public Category Category { get; set; }
        public string Raw_text { get; set; }
    }
}
