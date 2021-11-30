using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BLL.Services;
using DAL.EF;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Benchmark.Benchmarks
{
    //https://docs.microsoft.com/ru-ru/ef/core/performance/performance-diagnosis?tabs=simple-logging%2Cload-entities
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, targetCount: 1000)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class UpdateBenchmark
    {
        private AppDBContext _appDBContext;
        private QueryService _queryService;
        [GlobalSetup]
        public void GlobalSetup()
        {
            DbContextOptionsBuilder<AppDBContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            dbContextOptionsBuilder.UseMySql(Program.connection, new MySqlServerVersion(new Version(8, 0, 27)));
            _appDBContext = new AppDBContext(dbContextOptionsBuilder.Options);
            _queryService = new QueryService(_appDBContext);
        }

        private List<SchoolClass> Select()
        {
            return _appDBContext.SchoolClasses.Include(i => i.Pupils)
                .ThenInclude(j => j.PupilAcademicSubjects).ThenInclude(n => n.AcademicSubject)
                .ToList();
        }

        public static string GetUniqueKeyOriginal_BIASED(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private void Change(SchoolClass schoolClasse)
        {
            schoolClasse.Title = GetUniqueKeyOriginal_BIASED(4);
            schoolClasse.Pupils.First().FirstName = GetUniqueKeyOriginal_BIASED(8);
            schoolClasse.Pupils.First().PupilAcademicSubjects.First().AcademicSubject.Title 
                = GetUniqueKeyOriginal_BIASED(14);
        }

        [Benchmark]
        public void EFCoreUpdate()
        {
            var vs = Select();
            Change(vs[0]);
            var transaction = _appDBContext.Database.BeginTransaction();
            try
            {
                _appDBContext.Update(vs[0]);
                _appDBContext.SaveChanges();
                transaction.Commit();
            }
            catch
            { }
        }

        [Benchmark]
        public void MyUpdate()
        {
            var vs = Select();
            Change(vs[0]);
            _queryService.ParametricUpdate(vs[0], new[] { "Title", "Description" }, 
                new string[] { "Id", "PupilId", "AcademicSubjectId" });
        }

        [Benchmark]
        public void EFCoreUpdateRange()
        {
            var vs = Select();
            Change(vs[0]);
            Change(vs[1]);
            var transaction = _appDBContext.Database.BeginTransaction();
            try
            {
                _appDBContext.UpdateRange(vs);
                _appDBContext.SaveChanges();
                transaction.Commit();
            }
            catch { }
        }

        [Benchmark]
        public void MyUpdateRange()
        {
            var vs = Select();
            Change(vs[0]);
            Change(vs[1]);
            _queryService.ParametricUpdate(vs, new[] { "Title", "Description" },
                new string[] { "Id", "PupilId", "AcademicSubjectId" });
        }
    }
}
