﻿using BLL.Interfaces;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Text;
using DAL.Entity;
using MySqlConnector;

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
            string studySubject_ = "Chemistry";
            var pupilIds = _appDBContext.Pupils.AsNoTracking()
                .Include(i => i.PupilAcademicSubjects.Where(j => j.AcademicSubject.Title.ToLower() == studySubject_.ToLower()))
                .ToArray().Where(n => n.PupilAcademicSubjects.Count > 0).Select(j => j.Id).ToArray();
            var temp_ = _appDBContext.SchoolClasses.Include(i => i.Pupils.Where(j => pupilIds.Any(n => n == j.Id)))
                .ToArray();

            
            MySqlParameter studySubjectParameter = new MySqlParameter("@studySubject", studySubject_);

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
                            @"WHERE a0.Title = @studySubject " +
                            //@"WHERE a0.Title = 'Chemistry' " +
                        @") AS `a1` ON a1.aId = pas.AcademicSubjectId " +
                    @") AS `pas0` ON p.Id = pas0.PupilId " +
                @") AS `p1` ON sc.Id = p1.SchoolClassId";
            var schoolClass = _appDBContext.SchoolClasses.FromSqlRaw(query, studySubjectParameter)
                .Distinct().AsEnumerable();
            #endregion
            

        }

        public void DifficultUpdate()
        {
            string studySubject_ = "Chemistry";
            var schoolClasses = _appDBContext.SchoolClasses.Include(i => i.Pupils)
                .ThenInclude(j=>j.PupilAcademicSubjects).ThenInclude(n=>n.AcademicSubject)
                .ToList();
            string newDescription = "learn about chemical";
            AcademicSubject academicSubject = null;
            //schoolClasses.ForEach(i =>
            //{
            //    foreach (var pupil in i.Pupils)//ICollection does not have ForEach function
            //    {
            //         academicSubject = pupil.PupilAcademicSubjects
            //            .Where(i => i.AcademicSubject.Title.ToLower() == studySubject_.ToLower())
            //            .Select(n=>n.AcademicSubject).FirstOrDefault();
            //        if (academicSubject is not null)
            //            break;
            //    }
            //});

            academicSubject =  schoolClasses.Select(i => i.Pupils.Select(j => j.PupilAcademicSubjects
                  .Where(i => i.AcademicSubject.Title.ToLower() == studySubject_.ToLower())
                  .Select(k=>k.AcademicSubject).FirstOrDefault()).FirstOrDefault()).FirstOrDefault();

            if(academicSubject is not null)
            {
                academicSubject.Description = newDescription;
                _appDBContext.AcademicSubjects.Update(academicSubject);
                _appDBContext.SaveChanges();
            }
            
                //.FirstOrDefault().PupilAcademicSubjects.FirstOrDefault().AcademicSubject.Description
                //= "Count digits";

        }
    }
}
