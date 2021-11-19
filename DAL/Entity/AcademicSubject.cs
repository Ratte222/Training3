using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class AcademicSubject: BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<PupilAcademicSubject> PupilAcademicSubjects { get; set; }
    }
}
