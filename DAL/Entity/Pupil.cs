using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class Pupil:BaseEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }
        public ICollection<PupilAcademicSubject> PupilAcademicSubjects { get; set; }
    }
}
