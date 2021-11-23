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
            #endregion
            // Work in Heidi 
            //SELECT x0.Id, x0.Title, y0.Id, y0.FirstName, y0.LastName, y0.SchoolClassId, y1.PupilId, y1.AcademicSubjectId, z0.Id, z0.Title, z0.Description
            //FROM schoolclasses AS x0 LEFT JOIN pupils AS y0 ON x0.Id = y0.SchoolClassId
            //LEFT JOIN pupilacademicsubjects AS y1 ON y1.PupilId = y0.Id
            //LEFT JOIN academicsubjects AS z0 ON z0.Id = y1.AcademicSubjectId
            //WHERE z0.Title = 'Chemistry'
            
            string query = @"SELECT `sc`.`Id`, `sc`.`Title`, `s`.`Id0`, `s`.`FirstName`, `s`.`LastName`, `s`.`SchoolClassId` FROM ( "
                        + @"SELECT `p`.`Id` AS `Id0`, `p`.`FirstName`, `p`.`LastName`, `p`.`SchoolClassId` FROM pupils AS `p` "
                        + @"WHERE `p`.`FirstName` = 'Taya'"
                        + @") AS `s`"
                        + @"LEFT JOIN schoolclasses AS `sc` ON `sc`.`Id` = `s`.`SchoolClassId` ";


            query = @"SELECT sc.Id, sc.Title, p1.pId, p1.FirstName, p1.LastName, p1.SchoolClassId, " +
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
            //var temp = _appDBContext.SchoolClasses.FromSqlInterpolated($"SELECT `sc`.`Id`, `sc`.`Title`, `s`.`Id0`, `s`.`FirstName`, `s`.`LastName`, `s`.`SchoolClassId` FROM ( SELECT `p`.`Id` AS `Id0`, `p`.`FirstName`, `p`.`LastName`, `p`.`SchoolClassId` FROM pupils AS `p` WHERE `p`.`FirstName` = 'Taya' ) AS `s` LEFT JOIN schoolclasses AS `sc` ON `sc`.`Id` = `s`.`SchoolClassId`"
            //              ).ToArray();
            var schoolClass = _appDBContext.SchoolClasses.FromSqlRaw(query)
                .AsNoTracking().AsEnumerable()
                .GroupBy(i => i.Id
                    //j=>j, (key, entity)=>
                    //new {
                    //    SchoolClass = entity
                    //}
                );

            //var temp2 = _appDBContext.AcademicSubjects.Where(i => i.Title == "Chemistry")
            //    .Include(j => j.PupilAcademicSubjects).Select(n=>n.PupilAcademicSubjects.Select(i=>i.PupilId)).ToArray();
            //var temp4 = _appDBContext.SchoolClasses.Include(i => i.Pupils)
            //    /*.Where(j=> j.Pupils)*/.ToArray();
            

            //var temp5 = _appDBContext.Pupils.Where(j => j.FirstName == "Taya");
            //var temp3 = temp_.GroupBy(pup => pup.SchoolClass,
            //            pupi => pupi
            //            ).ToArray();
            //.Where(i => i.Pupils.( FirstName == "Taya").ToArray();
        }


    }
}
