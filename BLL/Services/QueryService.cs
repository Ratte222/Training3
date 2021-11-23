using BLL.Interfaces;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Text;
using DAL.Entity;

namespace BLL.Services
{
    public class QueryService: IQueryService
    {
        private readonly AppDBContext _appDBContext;
        public QueryService(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }


        //https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
        public void SQLQuery()
        {
            #region EF Core 5+
            var pupilIds = _appDBContext.Pupils.AsNoTracking()
                .Include(i => i.PupilAcademicSubjects.Where(j => j.AcademicSubject.Title.ToLower() == "history"))
                .ToArray().Where(n => n.PupilAcademicSubjects.Count > 0).Select(j=>j.Id).ToArray();
            var temp_ = _appDBContext.SchoolClasses.Include(i => i.Pupils.Where(j => pupilIds.Any(n => n == j.Id)))
                .ToArray();

            string query = @"SELECT sc.Id, sc.Title, p1.pId, p1.FirstName, p1.LastName, p1.SchoolClassId, " +
                @"p1.PupilId, p1.AcademicSubjectId, p1.aId, p1.aTitle, p1.Description " +
                @"FROM schoolclasses AS `sc` " +
                @"INNER JOIN ( " +
                @"SELECT p.Id AS pId, p.FirstName, p.LastName, p.SchoolClassId, pas0.PupilId, pas0.AcademicSubjectId, " +
                    @"pas0.aId, pas0.Title AS aTitle, pas0.Description FROM pupils AS `p` " +
                    @"INNER JOIN ( " +
                    @"SELECT pas.PupilId, pas.AcademicSubjectId, a1.aId, a1.Title, a1.Description FROM pupilacademicsubjects AS `pas` " +
                        @"INNER JOIN ( " +
                        @"SELECT a0.Id AS aId, a0.Title, a0.Description FROM academicsubjects AS `a0` " +
                            @"WHERE a0.Title = 'Chemistry' " +
                        @") AS `a1` ON a1.aId = pas.AcademicSubjectId " +
                    @") AS `pas0` ON p.Id = pas0.PupilId " +
                @") AS `p1` ON sc.Id = p1.SchoolClassId";
            var schoolClass = _appDBContext.SchoolClasses.FromSqlRaw(query)
                .AsNoTracking().AsEnumerable()
                .GroupBy(i => i.Id
                //j=>j, (key, entity)=>
                //new {
                //    SchoolClass = entity
                //}
                );
            #endregion


        }


    }
}
