using CommonForCryptPasswordLibrary.WorkWithJson;
using DAL_NS.Entity;
using NotificationService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NotificationService.Services
{
    public class JsonFileProviderService<TModel> : IFileProviderService<TModel>
    {
        private readonly string _pathToFile;

        public JsonFileProviderService(string pathToFile)
        {
            _pathToFile = pathToFile;
        }
        public List<TModel> ReadFromDisck()
        {
            if (!File.Exists(_pathToFile))
            {
                return new List<TModel>();
            }
            SerializeDeserializeJson<List<TModel>> serialize = new SerializeDeserializeJson<List<TModel>>();
            var model = serialize.DeserializeFromFile(_pathToFile);
            return model;
        }

        public void WriteToDisck(List<TModel> models)
        {            
            //if (models.Count > 0)
            //{
                SerializeDeserializeJson<List<TModel>> serialize = new SerializeDeserializeJson<List<TModel>>();
                serialize.SerializeToFile(models, _pathToFile);
            //}
        }
    }
}
