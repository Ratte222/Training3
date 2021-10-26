using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entity
{
    [Table("category")]
    public class Category:BaseEntity<int>
    {
        [StringLength(255), Column("codename")]
        public string Codename { get; set; }
        [StringLength(255), Column("name")]
        public string Name { get; set; }
        [Column("is_base_expense")]
        public bool Is_base_expense { get; set; }
        [Column("is_base_income")]
        public bool Is_base_income { get; set; }
        [Column("is_income")]
        public bool Is_income { get; set; }
        [Column("aliases")]
        public string Aliases { get; set; }
        public override string ToString()
        {
            //return base.ToString();
            return $"{Id}, {Codename}, {Name}, {Is_base_expense}," +
                $"{Is_base_income}, {Is_income}, {Aliases}";
        }
    }
}
