using Data.Persistence.Documents;
using MongoDB.Driver;

namespace Data.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<KundeDocument> Kunden { get; }
        IMongoCollection<RechnungDocument> Rechnungen { get; }
        IMongoCollection<LeistungDocument> Leistungen { get; }
        IMongoCollection<UserDocument> User { get; }
        IMongoCollection<FirmaDocument> Firmen { get; }
    }
}
