using Data.Configuration.MongoDB;
using Data.Persistence.Documents;
using MongoDB.Driver;

namespace Data.Persistence.Seeding
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly IMongoDatabase _database;

        public DatabaseSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedAsync()
        {
            await SeedCollectionAsync(CollectionNamen.Kunden, GetKundenSeedData());

            await SeedCollectionAsync(CollectionNamen.Firmen, GetFirmenSeedData());

            await SeedCollectionAsync(CollectionNamen.Leistungen, GetLeistungenSeedData());

            await SeedCollectionAsync(CollectionNamen.User, GetUserSeedData());
        }

        private async Task SeedCollectionAsync<T>(string collectionName, IEnumerable<T> seedData)
        {
            var collection = _database.GetCollection<T>(collectionName);

            if (await collection.CountDocumentsAsync(FilterDefinition<T>.Empty) > 0)
                return;

            await collection.InsertManyAsync(seedData);
        }

        private IEnumerable<KundeDocument> GetKundenSeedData()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "SeedData", "kunden_seed.json");
            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<KundeDocument>>(json)!;
        }

        private IEnumerable<FirmaDocument> GetFirmenSeedData()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "SeedData", "firmen_seed.json");
            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<FirmaDocument>>(json)!;
        }

        private IEnumerable<LeistungDocument> GetLeistungenSeedData()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "SeedData", "leistungen_seed.json");
            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<LeistungDocument>>(json)!;
        }

        private IEnumerable<UserDocument> GetUserSeedData()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "SeedData", "users_seed.json");
            var json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<UserDocument>>(json)!;
        }
    }
}
