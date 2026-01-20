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
    public class TerminRepository : BaseRepository<TerminDocument, Termin>, ITerminRepository
    {
        public TerminRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.Termine, mapper, logger)
        {

        }

        public async Task<PaginiertesResultDto<TerminDto>> GetPaginierteTermine(int seite, int eintraegeProSeite, string? teileingabe)
        {
            try
            {
                FilterDefinition<TerminDocument> filter = FilterDefinition<TerminDocument>.Empty;

                if (!string.IsNullOrWhiteSpace(teileingabe))
                {
                    var regex = new BsonRegularExpression(teileingabe, "i");
                    filter = Builders<TerminDocument>.Filter.Or(
                        Builders<TerminDocument>.Filter.Regex(t => t.Text, regex)
                    );
                }


                var gesamtAnzahl = (int)await _collection.CountDocumentsAsync(filter);
                var terminDocs = await _collection
                    .Find(filter)
                    .Skip((seite - 1) * eintraegeProSeite)
                    .Limit(eintraegeProSeite)
                    .ToListAsync();

                var termine = terminDocs.Select(k => _mapper.Map<TerminDto>(k)).ToList();
                return new PaginiertesResultDto<TerminDto>(termine, gesamtAnzahl, eintraegeProSeite, seite);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der gefilterten Termine aus der Datenbank.");
                throw new RepositoryException("Abruf der gefilterten Termine aus Datenbank fehlgeschlagen.", ex);
            }
        }
    }
}
