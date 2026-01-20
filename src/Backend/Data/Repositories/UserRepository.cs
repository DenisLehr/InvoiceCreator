using AutoMapper;
using Data.Configuration.MongoDB;
using Data.Interfaces;
using Data.Persistence.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.Dtos;
using Shared.Exceptions;

namespace Data.Repositories
{
    public class UserRepository : BaseRepository<UserDocument, User>, IUserRepository
    {
        public UserRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.User, mapper, logger)
        { }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                var userDoc = await _collection.FindAsync(u => u.Email == email);
                var result = _mapper.Map<User>(userDoc.FirstOrDefault());
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen des Kunden per Email.");
                throw new RepositoryException("Abruf des Kunden per Email aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<PaginiertesResultDto<UserDto>> GetPaginierteUser(int seite, int eintraegeProSeite, string? teileingabe)
        {
            try
            {
                FilterDefinition<UserDocument> filter = FilterDefinition<UserDocument>.Empty;

                if (!string.IsNullOrWhiteSpace(teileingabe))
                {
                    var regex = new BsonRegularExpression(teileingabe, "i");
                    filter = Builders<UserDocument>.Filter.Or(
                        Builders<UserDocument>.Filter.Regex(u => u.UserName, regex),
                        Builders<UserDocument>.Filter.Regex(u => u.Email, regex)
                    );
                }


                var gesamtAnzahl = (int)await _collection.CountDocumentsAsync(filter);
                var userDocs = await _collection
                    .Find(filter)
                    .Skip((seite - 1) * eintraegeProSeite)
                    .Limit(eintraegeProSeite)
                    .ToListAsync();

                var user = userDocs.Select(k => _mapper.Map<UserDto>(k)).ToList();
                return new PaginiertesResultDto<UserDto>(user, gesamtAnzahl, eintraegeProSeite, seite);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der gefilterten User aus der Datenbank.");
                throw new RepositoryException("Abruf der gefilterten User aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            try
            {
                var result = await _collection.FindAsync(u => u.Email == email);
                return await result.AnyAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Überprüfen der Email-Adresse.");
                throw new RepositoryException("Überprüfung vergebener Email-Adresse in Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            try
            {
                var result = await _collection.FindAsync(u => u.UserName == username);
                return await result.AnyAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Überprüfen des Usernamen.");
                throw new RepositoryException("Überprüfung des vergebenen Usernamen in Datenbank fehlgeschlagen.", ex);
            }
        }

    }
}
