using NotificationService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NotificationService.Services
{
    public class BinaryFileProviderService<TModel> : IFileProviderService<TModel>
    {
        private readonly string _pathToFile;

        public BinaryFileProviderService(string pathToFile)
        {
            _pathToFile = pathToFile;
        }

        public void WriteToDisck(List<TModel> models)
        {
            if(models.Count > 0)
            {
                using (FileStream fs = new FileStream(_pathToFile, FileMode.OpenOrCreate))
                {
                    // Construct a BinaryFormatter and use it to serialize the data to the stream.
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, models);
                }
            }            
        }

        public List<TModel> ReadFromDisck()
        {
            if(!File.Exists(_pathToFile))
            {
                return new List<TModel>();
            }
            using (FileStream fs = new FileStream(_pathToFile, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and
                // assign the reference to the local variable.
                var result = (List<TModel>)formatter.Deserialize(fs);
                return result;
            }
        }
    }
}
