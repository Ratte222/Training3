using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class SchoolClass:BaseEntity<int>
    {
        public string Title { get; set; }
        public ICollection<Pupil> Pupils { get; set; }
    }
}
