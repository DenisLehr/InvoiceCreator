using AutoMapper;
using Data.Configuration.MongoDB;
using Data.Interfaces;
using Data.Persistence.Documents;
using Data.Persistence.Enums;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Shared.Domain.Models;
using Shared.Dtos;
using Shared.Exceptions;

namespace Data.Repositories
{
    public class RechnungRepository : BaseRepository<RechnungDocument, Rechnung>, IRechnungRepository
    {
        public RechnungRepository(IMongoDatabase database, IMapper mapper, ILoggerFactory logger) : base(database, CollectionNamen.Rechnungen, mapper, logger)
        {
        }

        public async Task<List<Rechnung>> GetOffeneRechnungenAsync()
        {
            try
            {
                var offeneRechnungenDocs = await _collection.Find(r => r.Zahlungsstatus == ZahlungsstatusDocument.Offen.ToString()).ToListAsync();
                var rechnungen = offeneRechnungenDocs.Select(r => _mapper.Map<Rechnung>(r)).ToList();
                return rechnungen;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen offener Rechnungen.");
                throw new RepositoryException("Abruf der offenen Rechnungen aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public Task<PaginiertesResultDto<RechnungDto>> GetPaginierteRechnungen(int seite, int eintraegeProSeite, string? teileingabe)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Rechnung>> GetRechnungenImZeitraum(DateTime startDatum, DateTime endDatum)
        {
            try
            {
                var rechnungenDocs = await _collection.Find(r => r.Rechnungsdatum >= startDatum && r.Rechnungsdatum <= endDatum).ToListAsync();
                var rechnungen = rechnungenDocs.Select(r => _mapper.Map<Rechnung>(r)).ToList();
                return rechnungen;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Rechnungen im Zeitraum vom {startDatum.ToShortDateString()} bis zum {endDatum.ToShortDateString()}.");
                throw new RepositoryException("Abruf der Rechnungen aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<List<Rechnung>> GetRechnungenVonKundeAsync(string kundeId)
        {
            try
            {
                var rechnungenDocs = await _collection.Find(r => r.KundeID == kundeId).ToListAsync();
                var result = rechnungenDocs.Select(r => _mapper.Map<Rechnung>(r)).ToList();
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Rechnungen von Kunde {kundeId}.");
                throw new RepositoryException("Abruf der Rechnungen aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<Rechnung> GetRechnungMitBestellreferenzAsync(string bestellReferenz)
        {
            try
            {
                var rechnungDoc = await _collection.Find(r => r.Bestellreferenz == bestellReferenz).ToListAsync();
                var result = _mapper.Map<Rechnung>(rechnungDoc);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Rechnung mit Bestellreferenz {bestellReferenz}.");
                throw new RepositoryException("Abruf der Rechnung aus Datenbank fehlgeschlagen.", ex);
            }
        }

        public async Task<Rechnung> GetRechnungMitRechnungsNummerAsync(string rechnungsNummer)
        {
            try
            {
                var rechnungDoc = await _collection.Find(r => r.Rechnungsnummer == rechnungsNummer).ToListAsync();
                var result = _mapper.Map<Rechnung>(rechnungDoc);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, $"Fehler beim Abrufen der Rechnung mit Rechnungsnummer {rechnungsNummer}.");
                throw new RepositoryException("Abruf der Rechnung aus Datenbank fehlgeschlagen.", ex);
            }
        }
    }
}
