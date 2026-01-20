using AutoMapper;
using Data.Configuration.MongoDB;
using Data.Interfaces;
using Data.Persistence.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Exceptions;

namespace Data.Repositories
{
    public class FirmaRepository : BaseRepository<FirmaDocument,Firma>, IFirmaRepository
    {
        private static readonly ObjectId DefaultFirmaId = ObjectId.Parse("68fbc6a1a54472b463bf7bc6");
        public FirmaRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.Firmen, mapper, logger)
        {
        }

        public async Task<Firma> GetFirmendatenAsync()
        {
            try
            {
                var result = await _collection.FindAsync(f => f.Id == DefaultFirmaId.ToString());
                var firmaDoc = await result.FirstOrDefaultAsync();
                var firma = _mapper.Map<Firma>(firmaDoc);
                return firma;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Firmendaten aus Datenbank.");
                throw new RepositoryException("Abruf der Firmendaten aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<Adresse> GetAdresseVonFirmaAsync()
        {
            try
            {
                var result = await _collection.FindAsync(f => f.Id == DefaultFirmaId.ToString());
                var firmaDoc = result.FirstOrDefault();
                var firma = _mapper.Map<Firma>(firmaDoc);
                return firma?.Adresse;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Firmenadresse aus Datenbank.");
                throw new RepositoryException("Abruf der Firmenadresse aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<Bankverbindung> GetBankverbindungVonFirmaAsync()
        {
            try
            {
                var result = await _collection.FindAsync(f => f.Id == DefaultFirmaId.ToString());
                var firmaDoc = result.FirstOrDefault();
                var firma = _mapper.Map<Firma>(firmaDoc);
                return firma?.Bankverbindung;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Bankverbindung aus Datenbank.");
                throw new RepositoryException("Abruf der Bankverbindung aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task UpdateAdresseVonFirmaAsync(Adresse adresse)
        {
            try
            {
                var adresseDoc = _mapper.Map<AdresseDocument>(adresse);
                var filter = Builders<FirmaDocument>.Filter.Eq(f => f.Id, DefaultFirmaId.ToString());
                var update = Builders<FirmaDocument>.Update.Set(f => f.Adresse, adresseDoc);
                await _collection.UpdateOneAsync(filter, update);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Updaten der Firmenadresse in Datenbank.");
                throw new RepositoryException("Update der Firmenadresse in Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task UpdateBankverbindungVonFirmaAsync(Bankverbindung bankverbindung)
        {
            try
            {
                var bankverbindungDoc = _mapper.Map<BankverbindungDocument>(bankverbindung);
                var filter = Builders<FirmaDocument>.Filter.Eq(f => f.Id, DefaultFirmaId.ToString());
                var update = Builders<FirmaDocument>.Update.Set(f => f.Bankverbindung, bankverbindungDoc);
                await _collection.UpdateOneAsync(filter, update);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Update der Bankverbindung in Datenbank.");
                throw new RepositoryException("Update der Bankverbindung in Datenbank fehlgeschlagen.", ex);
            }
        }
    }
}
