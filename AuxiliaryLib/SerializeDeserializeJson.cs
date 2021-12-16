using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonForCryptPasswordLibrary.WorkWithJson
{
    public class SerializeDeserializeJson<T>
    {
        public T DeserializeFromFile(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamReader sr = new StreamReader(path))
            using (JsonReader readed = new JsonTextReader(sr))
            {
                return (T)serializer.Deserialize(sr, typeof(T));
            }
        }

        public void SerializeToFile(T jsonData, string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.IndentChar = ' ';
                serializer.Serialize(writer, jsonData);
            }
        }

        public T Deserialize(string jsonData)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
            jsonSetting.NullValueHandling = NullValueHandling.Ignore;
            jsonSetting.Formatting = Formatting.Indented;
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return (T)JsonConvert.DeserializeObject(jsonData, typeof(T), jsonSetting);
        }

        public string Serialize(T jsonData)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
            jsonSetting.NullValueHandling = NullValueHandling.Ignore;
            jsonSetting.Formatting = Formatting.Indented;
            jsonSetting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return JsonConvert.SerializeObject(jsonData, jsonSetting);
        }
    }
}
