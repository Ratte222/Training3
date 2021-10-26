using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entity
{
    [Table("expense")]
    public class Expense:BaseEntity<int>
    {
        [Column("amount")]
        public int Amount { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("category_codename")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Column("raw_text")]
        public string Raw_text { get; set; }
        

        public override string ToString()
        {
            //return base.ToString();
            return $"{Id}, {Amount}, {Created}, {CategoryId}, {Raw_text}";
        }
    }
}
