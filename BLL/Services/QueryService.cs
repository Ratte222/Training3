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
            string studySubject = "Chemistry";
            var pupilsIds = _appDBContext.AcademicSubjects
                .Where(i => i.Title.ToLower() == studySubject.ToLower())
                .Include(j => j.PupilAcademicSubjects)
                .Select(i=>i.PupilAcademicSubjects.Select(n=>n.PupilId)).ToArray();
            //bool temp = pupilsIds[0].Any(i => i == 5);
            if(pupilsIds.Length > 0)
            {
                var schoolClasses = (from sc in _appDBContext.SchoolClasses
                                     orderby sc.Id
                                     join p in _appDBContext.Pupils on sc.Id equals p.SchoolClassId
                                     where sc.Id == p.SchoolClassId && pupilsIds[0].Any(i => i == p.Id)
                                     select sc).Distinct().ToArray();
            }            
        }


    }
}
