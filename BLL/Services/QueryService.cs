using BLL.Interfaces;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Text;
using AuxiliaryLib.Extensions;
using DAL.Entity;
using MySqlConnector;
using System.Reflection;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class QueryService: IQueryService
    {
        private readonly AppDBContext _appDBContext;
        //private readonly ILogger<QueryService> _logger;
        public QueryService(AppDBContext appDBContext/*, ILogger<QueryService> logger*/)
        {
            _appDBContext = appDBContext;
            //_logger = logger;
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
            string newDescription = "do not learn about chemical";
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
            #region Tests
            //if (academicSubject is not null)
            //    academicSubject.Description = newDescription;
            //ParametricUpdate(academicSubject, new[] { "Description" });

            //schoolClasses[0].Title = "6r";
            //schoolClasses[0].Pupils.First().FirstName = "Igor_";
            //schoolClasses[0].Pupils.First().PupilAcademicSubjects.First().AcademicSubject.Title = "test123";

            //_appDBContext.Update(schoolClasses[0]);
            //_appDBContext.SaveChanges();
            //ParametricUpdate(schoolClasses[0], new[] { "Title" }, new string[] { "Id", "PupilId", "AcademicSubjectId" });
            #endregion

            if (academicSubject is not null)
            {

                _appDBContext.AcademicSubjects.Update(academicSubject);
                _appDBContext.SaveChanges();
            }

            if (academicSubject is not null)
            {

                _appDBContext.AcademicSubjects.Attach(academicSubject);
                _appDBContext.Entry(academicSubject).Property(i => i.Description).IsModified = true;
                academicSubject.Description = newDescription;
                _appDBContext.SaveChanges();
            }



        }

        public TEntity ParametricUpdate<TEntity>(TEntity item, string[] parameterNames) where TEntity : class
        {
            if (item is null)
                return null;
            _appDBContext.Set<TEntity>().Attach(item);
            var type = typeof(TEntity);
            var instanceName = type.GetAllPublicProperty();
            foreach (var parameterName in parameterNames)
            {
                if(instanceName.Any(i=>i.Equals(parameterName)))
                    _appDBContext.Entry(item).Property(parameterName).IsModified = true;
            }
            var propertyInfos = type.GetAllPublicCollection();
            foreach (var propertyInfo in propertyInfos)
            {
                var pT = propertyInfo.PropertyType;
                var tCast = typeof(ICollection<>);
                if(pT.IsGenericType && tCast.IsAssignableFrom(pT.GetGenericTypeDefinition()) ||
                    pT.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tCast))
                {
                    IEnumerable listObject = (IEnumerable)propertyInfo.GetValue(item, null);
                    if (listObject != null)
                    {
                        //_appDBContext.UpdateRange(listObject);
                        foreach (object o in listObject)
                        {
                            //Type t = o.GetType();
                            //_appDBContext.Update(o);
                            _appDBContext.Attach(o);

                        }
                    }
                }                
            }
            _appDBContext.SaveChanges();
            return item;
        }

        public void ParametricUpdate(object item, string[] parameterNames, string[] idsName = null)
        {
            _ = item ?? throw new NullReferenceException($"{nameof(item)} is null");
            _ = parameterNames ?? throw new NullReferenceException($"{nameof(parameterNames)} is null");
            idsName ??=  new string[] { "Id" };
            var transaction = _appDBContext.Database.BeginTransaction();
            try
            {
                var type = item.GetType();
                var tCast = typeof(IEnumerable);
                if (type.IsGenericType && tCast.IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                    type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tCast))
                {
                    IEnumerable listObject = (IEnumerable)item;
                    if (listObject != null)
                    {
                        foreach (object o in listObject)
                        {
                            _ParametricUpdate(o, parameterNames, idsName);
                        }
                    }
                }
                else
                {
                    _ParametricUpdate(item, parameterNames, idsName);
                }
                transaction.Commit();
            }
            catch(Exception  ex)
            {
                //_logger.LogWarning(ex, "ParametricUpdate");
                throw ex;
            }
        }
        private void _ParametricUpdate(object item, string[] parameterNames, string[] idsName)
        {            
            _appDBContext.Attach(item);
            var type = item.GetType();
            var instanceName = type.GetAllPublicProperty().Except(
                type.GetAllPublicCollection().Select(i => i.Name).ToArray())
                .Except(idsName);
            foreach (var parameterName in parameterNames)
            {
                if (instanceName.Any(i => i.Equals(parameterName)))
                {
                    var itemParam = type.GetProperty(parameterName).GetValue(item, null);
                    if (itemParam is not null)
                    {
                        if(!itemParam.GetType().IsClass)
                            _appDBContext.Entry(item).Property(parameterName).IsModified = true;
                    }
                }
            }
            var propertyInfos = type.GetAllPublicCollection();
            foreach (var propertyInfo in propertyInfos)
            {
                var pT = propertyInfo.PropertyType;
                var tCast = typeof(ICollection<>);
                if (pT.IsGenericType && tCast.IsAssignableFrom(pT.GetGenericTypeDefinition()) ||
                    pT.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tCast))
                {
                    IEnumerable listObject = (IEnumerable)propertyInfo.GetValue(item, null);
                    if (listObject != null)
                    {
                        foreach (object o in listObject)
                        {
                            _ParametricUpdate(o, o.GetType().GetAllPublicProperty().ToArray(), idsName);
                        }
                    }
                }
            }
            _appDBContext.SaveChanges();
        }
    }
}
