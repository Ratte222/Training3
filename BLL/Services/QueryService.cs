using BLL.Interfaces;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Text;

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

            //string query = $"SELECT x0.Id, x0.Title " +
            //    $" FROM " +
            //    $"{nameof(_appDBContext.SchoolClasses)} AS x0 LEFT JOIN pupils AS y0 ON x0.Id = y0.SchoolClassId " +
            //    $"INNER JOIN ( " +
            //    $"SELECT y0.Id, y0.FirstName FROM {nameof(_appDBContext.PupilAcademicSubjects)} " +
            //    $"LEFT JOIN {nameof(_appDBContext.AcademicSubjects)} AS z0 ON z0.Id = y1.AcademicSubjectId " +
            //    $"WHERE z0.Title = " +
            //    $"'Chemistry' )  AS y1 ON y1.PupilId = y0.Id";
            //$"@academicSubjectsName";
            //SqlParameter academivSubjectName = new SqlParameter("@academicSubjectName", "Cemistry");

            string query = @"SELECT `sc`.`Id`, `sc`.`Title`, `s`.`Id0`, `s`.`FirstName`, `s`.`LastName`, `s`.`SchoolClassId` FROM ( "
                        + @"SELECT `p`.`Id` AS `Id0`, `p`.`FirstName`, `p`.`LastName`, `p`.`SchoolClassId` FROM pupils AS `p` "
                        + @"WHERE `p`.`FirstName` = 'Taya'"
                        + @") AS `s`"
                        + @"LEFT JOIN schoolclasses AS `sc` ON `sc`.`Id` = `s`.`SchoolClassId`";

            //this query generate ef core
            //query = @"SELECT `s`.`Id`, `s`.`Title`, `p`.`Id`, `p`.`FirstName`, `p`.`LastName`, `p`.`SchoolClassId` " +
            //    @"FROM `SchoolClasses` AS `s` " +
            //    @"LEFT JOIN `Pupils` AS `p` ON `s`.`Id` = `p`.`SchoolClassId` " +
            //    @"WHERE EXISTS ( " +
            //        @"SELECT 1 " +
            //        @"FROM `Pupils` AS `p0` " +
            //        @"WHERE (`s`.`Id` = `p0`.`SchoolClassId`) AND (`p0`.`FirstName` = 'Taya')) " +
            //    @"ORDER BY `s`.`Id`, `p`.`Id`";//Exception: An item with the same key has already been added. Key: Id'

            var temp = _appDBContext.SchoolClasses.FromSqlRaw(query).ToArray();
            //var temp = _appDBContext.Pupils.FromSqlRaw(query).ToList();


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
