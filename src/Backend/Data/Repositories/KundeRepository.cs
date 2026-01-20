using AutoMapper;
using Data.Configuration.MongoDB;
using Data.Interfaces;
using Data.Persistence.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;
using Shared.Exceptions;

namespace Data.Repositories
{
    public class KundeRepository : BaseRepository<KundeDocument, Kunde>, IKundeRepository
    {
        public KundeRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.Kunden, mapper, logger)
        {
        }

        public async Task<Adresse> GetAdresseVonKundeAsync(string id)
        {
            try
            {
                var kundeDoc = await _collection.Find(k => k.Id == id).FirstOrDefaultAsync();
                var kunde = _mapper.Map<Kunde>(kundeDoc);
                return kunde?.Adresse;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Adresse von Kunde {id}.");
                throw new RepositoryException("Abruf der Adresse aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<List<KundeDto>> GetKundenNamenAsync(string teileingabe)
        {
            try
            {
                var regex = new BsonRegularExpression(teileingabe, "i");

                var kundenDoc = await _collection
                    .Find(Builders<KundeDocument>.Filter.Or(
                        Builders<KundeDocument>.Filter.Regex(k => k.Vorname, regex),
                        Builders<KundeDocument>.Filter.Regex(k => k.Nachname, regex)
                    ))
                    .Limit(10)
                    .ToListAsync();
                var kunden = kundenDoc.Select(k => _mapper.Map<KundeDto>(k)).ToList();
                return kunden;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Kundennamen aus der Datenbank.");
                throw new RepositoryException("Abruf der Kundennamen aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<PaginiertesResultDto<KundeDto>> GetPaginierteKunden(int seite, int eintraegeProSeite, string? teileingabe)
        {
            try
            {
                FilterDefinition<KundeDocument> filter = FilterDefinition<KundeDocument>.Empty;

                if (!string.IsNullOrWhiteSpace(teileingabe))
                {
                    var regex = new BsonRegularExpression(teileingabe, "i");
                    filter = Builders<KundeDocument>.Filter.Or(
                        Builders<KundeDocument>.Filter.Regex(k => k.Vorname, regex),
                        Builders<KundeDocument>.Filter.Regex(k => k.Nachname, regex)
                    );
                }


                var gesamtAnzahl = (int) await _collection.CountDocumentsAsync(filter);
                var kundenDocs = await _collection
                    .Find(filter)
                    .Skip((seite-1)* eintraegeProSeite)
                    .Limit(eintraegeProSeite)
                    .ToListAsync();

                var kunden = kundenDocs.Select(k => _mapper.Map<KundeDto>(k)).ToList();
                return new PaginiertesResultDto<KundeDto>(kunden, gesamtAnzahl,eintraegeProSeite,seite);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der gefilterten Kunden aus der Datenbank.");
                throw new RepositoryException("Abruf der gefilterten Kunden aus Datenbank fehlgeschlagen.", ex);
            }
        }
    }
}
