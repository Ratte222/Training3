using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class PupilAcademicSubject
    {
        public int PupilId { get; set; }
        public Pupil Pupil { get; set; }

        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
    }
}
