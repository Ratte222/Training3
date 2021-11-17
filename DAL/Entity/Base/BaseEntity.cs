using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity.Base
{
    public class BaseEntity<T>
    {
        //https://www.thecodebuzz.com/serializer-objectserializer-is-not-configurable-bsonrepresentationattribute/
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public T Id { get; set; }
    }
}
