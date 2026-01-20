using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Persistence.Documents
{
    public class BaseDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime ErstelltAm { get; set; }
        public DateTime GeaendertAm { get; set; }
        public bool IstGeloescht { get; set; }
    }
}
