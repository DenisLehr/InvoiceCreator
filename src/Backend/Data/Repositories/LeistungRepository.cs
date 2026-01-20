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
    public class LeistungRepository : BaseRepository<LeistungDocument,Leistung>, ILeistungRepository
    {
        public LeistungRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.Leistungen, mapper, logger)
        {
        }

        public async Task<string?> GetLetzterLeistungCodeAsync()
        {
            var sort = Builders<LeistungDocument>.Sort.Descending(ld => ld.Code);

            var letzter = await _collection
                .Find(FilterDefinition<LeistungDocument>.Empty)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefaultAsync();

            return letzter?.Code;
        }

        public async Task<PaginiertesResultDto<LeistungDto>> GetPaginierteLeistungen(int seite, int eintraegeProSeite, string? teileingabe)
        {
            try
            {
                FilterDefinition<LeistungDocument> filter = FilterDefinition<LeistungDocument>.Empty;

                if (!string.IsNullOrWhiteSpace(teileingabe))
                {
                    var regex = new BsonRegularExpression(teileingabe, "i");
                    filter = Builders<LeistungDocument>.Filter.Or(
                        Builders<LeistungDocument>.Filter.Regex(k => k.Code, regex),
                        Builders<LeistungDocument>.Filter.Regex(k => k.Bezeichnung, regex)
                    );
                }


                var gesamtAnzahl = (int)await _collection.CountDocumentsAsync(filter);
                var leistungDocs = await _collection
                    .Find(filter)
                    .Skip((seite - 1) * eintraegeProSeite)
                    .Limit(eintraegeProSeite)
                    .ToListAsync();

                var leistungen = leistungDocs.Select(k => _mapper.Map<LeistungDto>(k)).ToList();
                return new PaginiertesResultDto<LeistungDto>(leistungen, gesamtAnzahl, eintraegeProSeite, seite);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der gefilterten Leistungen aus der Datenbank.");
                throw new RepositoryException("Abruf der gefilterten Leistungen aus Datenbank fehlgeschlagen.", ex);
            }
        }
    }
}
