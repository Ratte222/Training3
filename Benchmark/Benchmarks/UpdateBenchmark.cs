using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using DAL.EF;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class UpdateBenchmark
    {
        private AppDBContext _appDBContext;
        [GlobalSetup]
        public void GlobalSetup()
        {
            DbContextOptionsBuilder<AppDBContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            dbContextOptionsBuilder.UseMySql(Program.connection, new MySqlServerVersion(new Version(8, 0, 27)));
            _appDBContext = new AppDBContext(dbContextOptionsBuilder.Options);

        }

        [Benchmark]
        public void DifficultUpdate1()
        {
            string studySubject_ = "Chemistry";
            var schoolClasses = _appDBContext.SchoolClasses.Include(i => i.Pupils)
                .ThenInclude(j=>j.PupilAcademicSubjects).ThenInclude(n=>n.AcademicSubject)
                .ToList();
            string newDescription = "learn about chemical";
            AcademicSubject academicSubject = null;
            schoolClasses.ForEach(i =>
            {
                foreach (var pupil in i.Pupils)//ICollection does not have ForEach function
                {
                    academicSubject = pupil.PupilAcademicSubjects
                       .Where(i => i.AcademicSubject.Title.ToLower() == studySubject_.ToLower())
                       .Select(n => n.AcademicSubject).FirstOrDefault();
                    if (academicSubject is not null)
                        break;
                }
            });
            if (academicSubject is not null)
            {
                academicSubject.Description = newDescription;
                _appDBContext.AcademicSubjects.Update(academicSubject);
                _appDBContext.SaveChanges();
            }
        }

        [Benchmark]
        public void DifficultUpdate2()
        {
            string studySubject_ = "Chemistry";
            var schoolClasses = _appDBContext.SchoolClasses.Include(i => i.Pupils)
                .ThenInclude(j => j.PupilAcademicSubjects).ThenInclude(n => n.AcademicSubject)
                .ToList();
            string newDescription = "learn about chemical";
            AcademicSubject academicSubject = academicSubject = schoolClasses.Select(i => i.Pupils.Select(j => j.PupilAcademicSubjects
                 .Where(i => i.AcademicSubject.Title.ToLower() == studySubject_.ToLower())
                 .Select(k => k.AcademicSubject).FirstOrDefault()).FirstOrDefault()).FirstOrDefault();
            if (academicSubject is not null)
            {
                academicSubject.Description = newDescription;
                _appDBContext.AcademicSubjects.Update(academicSubject);
                _appDBContext.SaveChanges();
            }
        }
    }
}
