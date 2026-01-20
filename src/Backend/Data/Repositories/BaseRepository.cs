using AutoMapper;
using Data.Interfaces;
using Data.Persistence.Documents;
using Microsoft.Extensions.Logging;
using Shared.Domain.Models;
using MongoDB.Driver;
using Shared.Exceptions;

namespace Data.Repositories
{
    public class BaseRepository<TDocument, TDomain> : IBaseRepository<TDomain> where TDocument : BaseDocument where TDomain : BaseModel
    {
        protected readonly IMongoCollection<TDocument> _collection;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public BaseRepository(IMongoDatabase database, string collectionName, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _collection = database.GetCollection<TDocument>(collectionName);
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger($"{typeof(BaseRepository<TDocument, TDomain>).FullName}");
        }

        public async Task<bool> DeleteAsync(TDomain entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                throw new ArgumentException("Die ID der Entität darf nicht leer sein.");

            try 
            {
                var filter = Builders<TDocument>.Filter.Eq(e => e.Id, entity.Id);
                var result = await _collection.DeleteOneAsync(filter);
                return result.DeletedCount == 1;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen Entität");
                throw new RepositoryException("Löschen der Entität fehlgeschlagen.", ex);
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Die ID darf nicht leer sein.");

            var result = await _collection.FindAsync(e => e.Id == id);
            return await result.AnyAsync();
        }

        public async Task<List<TDomain>> GetAllAsync()
        {
            try
            {
                var docs = await _collection.Find(_ => true).ToListAsync();

                if (!docs.Any())
                    throw new NotFoundException("Entitäten wurden nicht gefunden");

                var result = docs.Select(d => _mapper.Map<TDomain>(d)).ToList();
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Entitäten");
                throw new RepositoryException("Fehler beim Laden der Entitäten.", ex);
            }
        }

        public async Task<TDomain> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Die ID darf nicht leer sein.");

            try
            {
                var doc = await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

                if (doc == null)
                    throw new NotFoundException("Entität wurde nicht gefunden");
               
                var result = _mapper.Map<TDomain>(doc);
                return result;
            }
            catch (MongoException ex) 
            {
                _logger.LogError(ex, "Fehler beim Laden der Entität mit der ID {Id}", id);
                throw new RepositoryException("Fehler beim Laden der Entität.", ex);
            }
            
        }

        public async Task<bool> AddAsync(TDomain entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "Keine Entität übergeben.");

            try
            {
                entity.ErstelltAm = DateTime.UtcNow;
                var doc = _mapper.Map<TDocument>(entity);
                await _collection.InsertOneAsync(doc);
                return true;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Anlegen Entität");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TDomain entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                throw new ArgumentException("Die ID der Entität darf nicht leer sein.");
            
            try
            {
                entity.GeaendertAm = DateTime.UtcNow;
                var doc = _mapper.Map<TDocument>(entity);
                var filter = Builders<TDocument>.Filter.Eq(e => e.Id, entity.Id);
                var result = await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
                return result.IsAcknowledged && (result.ModifiedCount == 1 || result.UpsertedId != null);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der {EntityType} mit ID {Id}",typeof(TDomain).Name ,entity.Id);
                throw new RepositoryException("Update fehlgeschlagen.", ex);
            }
        }

        public async Task<int> CountAsync()
        {
            var count = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);
            return (int)count;
        }
    }
}
