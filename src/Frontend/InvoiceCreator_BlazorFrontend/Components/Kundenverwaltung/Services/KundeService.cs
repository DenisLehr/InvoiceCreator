using InvoiceCreator_BlazorFrontend.Components.Common.Exceptions;
using InvoiceCreator_BlazorFrontend.Components.Kundenverwaltung.Mapper;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Kundenverwaltung.Services
{
    public class KundeService
    {
        private readonly HttpClient _client;
        private readonly ILogger<KundeService> _logger;

        public KundeService(HttpClient client, ILogger<KundeService> logger) 
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Schickt einen Get-Request für alle Kunden an die Haupt-API
        /// </summary>
        /// <returns>Liste aller Kunden</returns>
        /// <exception cref="Exception">Fehler beim Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<List<Kunde>> GetAllKundenAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<BaseResponse<List<KundeDto>>>("kunde");
                var kunden = response.Daten.Select(k => KundeMapper.FromKundeDto(k)).ToList();
                return kunden ?? throw new Exception("Laden der Kundenliste in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Get-Request für einen einzelnen Kunden an die Haupt-API
        /// </summary>
        /// <param name="id">einzigartige Id des angeforderten Kunden, die von MongoDB vergeben wird</param>
        /// <returns>Gewünschter Kunde</returns>
        /// <exception cref="Exception">Fehler beim  Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<Kunde> GetKundeByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Keine Id übermittelt.");
            }

            try
            {
                var result = await _client.GetFromJsonAsync<BaseResponse<KundeDto>>($"kunde/{id}");
                var kunde = KundeMapper.FromKundeDto(result.Daten); 

                return kunde ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Delete Request an die Haupt-API
        /// </summary>
        /// <param name="id">Id des zu löschenden Kunden</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Löschen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> DeleteKundeAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BaseResponse<bool>
                {  
                    Erfolg = false,
                    Daten = false,
                    Hinweis = "fehlende Id", 
                    Zeitstempel = DateTime.UtcNow 
                };
            }

            try
            {
                var httpResponse = await _client.DeleteAsync($"kunde/{id}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Löschen fehlgeschlagen für Kunde-ID: {Id}, StatusCode: {StatusCode}", id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Löschen fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                 
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Löschen des Kunden");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Daten = false,
                    Hinweis = "Interner Fehler beim Verarbeiten der Antwort.",
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Lädt Kunden je nach Auswahl der Einträge pro Seite und mit Suchfilter
        /// </summary>
        /// <param name="teileingabe">Suchbegriff für den Filter</param>
        /// <param name="seite">Gewünschte Seite</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>Kunden und Informationen zur maximalen Anzahl aller Seiten für die Auswahlbuttons</returns>
        /// <exception cref="Exception">Fehler bei der Übertragung aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<PaginiertesResultDto<Kunde>> GetPagedKundenAsync(string teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var query = $"kunde/paged?seite={seite}&eintraegeProSeite={eintraegeProSeite}";

                if (!string.IsNullOrWhiteSpace(teileingabe))
                    query += $"&teileingabe={Uri.EscapeDataString(teileingabe)}";

                var kundenDtos = await _client.GetFromJsonAsync<PaginiertesResultDto<KundeDto>>(query);
                var kunden = kundenDtos.DtoListe.Select(k => KundeMapper.FromKundeDto(k)).ToList();
                var response = new PaginiertesResultDto<Kunde>(kunden,kundenDtos.AnzahlSeiten, kundenDtos.ElementeProSeite,kundenDtos.AktuelleSeite);
                return response ?? throw new Exception("Laden der paginierten Kundenliste in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Post-Request an die Haupt-API
        /// </summary>
        /// <param name="kunde">Neu erstellter Kunde</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Erstellen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> CreateKundeAsync(Kunde kunde)
        {
            try
            {

                var httpResponse = await _client.PostAsJsonAsync("kunde",KundeMapper.ToCreateDto(kunde));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    
                    return new BaseResponse<bool>
                    {  
                        Erfolg = false, 
                        Daten = false,
                        Hinweis = "Erstellen des Kunden fehlgeschlagen", 
                        Zeitstempel = DateTime.UtcNow 
                    };
                }
                   
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Erstellen des Kunden");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Daten = false,
                    Hinweis = "Interner Fehler beim Verarbeiten der Antwort.",
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Schickt einen Put-Request an die Haupt-API
        /// </summary>
        /// <param name="kunde">Aktualisierter Kunde</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Aktualisieren der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> UpdateKundeAsync(Kunde kunde)
        {
            try
            {
                var httpResponse = await _client.PutAsJsonAsync($"kunde/{kunde.Id}", KundeMapper.ToUpdateDto(kunde));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Aktualisieren fehlgeschlagen für Kunde-ID: {Id}, StatusCode: {StatusCode}", kunde.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {  
                        Erfolg = false, 
                        Daten = false,
                        Hinweis = "Aktualisieren des Kunden fehlgeschlagen", 
                        Zeitstempel = DateTime.UtcNow 
                    };
                }
                
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Kunden");
                throw new ServiceUnavailableException("Der Kundenservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Aktualisieren des Kunden");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Daten = false,
                    Hinweis = "Interner Fehler beim Verarbeiten der Antwort.",
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }
    }
}
