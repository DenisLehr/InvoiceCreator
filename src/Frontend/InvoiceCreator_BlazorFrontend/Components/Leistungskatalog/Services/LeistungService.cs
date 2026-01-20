using InvoiceCreator_BlazorFrontend.Components.Common.Exceptions;
using InvoiceCreator_BlazorFrontend.Components.Leistungskatalog.Mapper;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Leistungskatalog.Services
{
    public class LeistungService
    {
        private readonly HttpClient _client;
        private readonly ILogger<LeistungService> _logger;

        public LeistungService(HttpClient client, ILogger<LeistungService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Schickt einen Get-Request für alle Leistungen an die Haupt-API
        /// </summary>
        /// <returns>Liste aller Leistungen</returns>
        /// <exception cref="Exception">Fehler beim Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<List<Leistung>> GetAllLeistungenAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<BaseResponse<List<LeistungDto>>>("leistung");
                var leistungen = response.Daten.Select(k => LeistungMapper.FromLeistungDto(k)).ToList();
                return leistungen ?? throw new Exception("Laden der Leistungenliste in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Leistungen");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Get-Request für eine einzelne Leistung an die Haupt-API
        /// </summary>
        /// <param name="id">einzigartige Id der angeforderten Leistung, die von MongoDB vergeben wird</param>
        /// <returns>Gewünschte Leistung</returns>
        /// <exception cref="Exception">Fehler beim  Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<Leistung> GetLeistungByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Keine Id übermittelt.");
            }

            try
            {
                var result = await _client.GetFromJsonAsync<BaseResponse<LeistungDto>>($"leistung/{id}");
                var leistung = LeistungMapper.FromLeistungDto(result.Daten);

                return leistung ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Leistung");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Delete Request an die Haupt-API
        /// </summary>
        /// <param name="id">Id der zu löschenden Leistung</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Löschen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> DeleteLeistungAsync(string id)
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
                var httpResponse = await _client.DeleteAsync($"leistung/{id}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Löschen fehlgeschlagen für Leistung-ID: {Id}, StatusCode: {StatusCode}", id, httpResponse.StatusCode);
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
                _logger.LogError(ex, "Fehler beim Löschen der Leistung");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Löschen der Leistung");
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
        public async Task<PaginiertesResultDto<Leistung>> GetPagedLeistungenAsync(string teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var query = $"leistung/paged?seite={seite}&eintraegeProSeite={eintraegeProSeite}";

                if (!string.IsNullOrWhiteSpace(teileingabe))
                    query += $"&teileingabe={Uri.EscapeDataString(teileingabe)}";

                var leistungDtos = await _client.GetFromJsonAsync<PaginiertesResultDto<LeistungDto>>(query);
                var leistungen = leistungDtos.DtoListe.Select(k => LeistungMapper.FromLeistungDto(k)).ToList();
                var response = new PaginiertesResultDto<Leistung>(leistungen, leistungDtos.AnzahlSeiten, leistungDtos.ElementeProSeite, leistungDtos.AktuelleSeite);
                return response ?? throw new Exception("Laden der paginierten Leistungenliste in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Leistungen");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Post-Request an die Haupt-API
        /// </summary>
        /// <param name="leistung">Neu erstellte Leistung</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Erstellen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> CreateLeistungAsync(Leistung leistung)
        {
            try
            {
                var httpResponse = await _client.PostAsJsonAsync("leistung", LeistungMapper.ToCreateDto(leistung));

                if (!httpResponse.IsSuccessStatusCode)
                {

                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Erstellen der Leistung fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Leistung");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Erstellen der Leistung");
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
        /// <param name="leistung">Aktualisierte Leistung</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Aktualisieren der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> UpdateLeistungAsync(Leistung leistung)
        {
            try
            {
                var httpResponse = await _client.PutAsJsonAsync($"leistung/{leistung.Id}", LeistungMapper.ToUpdateDto(leistung));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Aktualisieren fehlgeschlagen für Leistung-ID: {Id}, StatusCode: {StatusCode}", leistung.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Aktualisieren der Leistung fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der Leistung");
                throw new ServiceUnavailableException("Der Leistungservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Aktualisieren der Leistung");
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
