using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(AppDBContext context,  List<Category> categories,
            List<Expense> expenses, List<SchoolClass> schoolClasses, List<Pupil> pupils,
            List<AcademicSubject> academicSubjects, List<PupilAcademicSubject> pupilAcademicSubjects)
        {
            if(!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
            if(!context.Expenses.Any())
            {
                context.AddRange(expenses);
                context.SaveChanges();
            }
            if(!context.SchoolClasses.Any())
            {
                context.SchoolClasses.AddRange(schoolClasses);
                context.SaveChanges();
                context.Pupils.AddRange(pupils);
                context.AcademicSubjects.AddRange(academicSubjects);
                context.SaveChanges();
                context.PupilAcademicSubjects.AddRange(pupilAcademicSubjects);
                context.SaveChanges();
               
            }
        }
    }
}
