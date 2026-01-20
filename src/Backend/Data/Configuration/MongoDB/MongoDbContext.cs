using Data.Interfaces;
using Data.Persistence.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Data.Configuration.MongoDB
{
    public class MongoDbContext : IMongoDbContext
    {
        public IMongoDatabase Database { get; }

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<KundeDocument> Kunden => Database.GetCollection<KundeDocument>(CollectionNamen.Kunden);
        public IMongoCollection<RechnungDocument> Rechnungen => Database.GetCollection<RechnungDocument>(CollectionNamen.Rechnungen);
        public IMongoCollection<LeistungDocument> Leistungen => Database.GetCollection<LeistungDocument>(CollectionNamen.Leistungen);
        public IMongoCollection<UserDocument> User => Database.GetCollection<UserDocument>(CollectionNamen.User);
        public IMongoCollection<FirmaDocument> Firmen => Database.GetCollection<FirmaDocument>(CollectionNamen.Firmen);
        public IMongoCollection<TerminDocument> Termine => Database.GetCollection<TerminDocument>(CollectionNamen.Termine);

    }

    public static class CollectionNamen
    {
        public const string Kunden = "Kunden";
        public const string Rechnungen = "Rechnungen";
        public const string Leistungen = "Leistungen";
        public const string User = "User";
        public const string Firmen = "Firmen";
        public const string Termine = "Termine";
    }
}
