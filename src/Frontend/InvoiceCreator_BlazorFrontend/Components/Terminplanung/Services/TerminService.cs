using InvoiceCreator_BlazorFrontend.Components.Common.Exceptions;
using InvoiceCreator_BlazorFrontend.Components.Terminplanung.Mapper;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Terminplanung.Services
{
    public class TerminService
    {
        private readonly HttpClient _client;
        private readonly ILogger<TerminService> _logger;

        public TerminService(HttpClient client, ILogger<TerminService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Schickt einen Get-Request für alle Termine an die Haupt-API
        /// </summary>
        /// <returns>Liste aller Termine</returns>
        /// <exception cref="Exception">Fehler beim Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<List<Termin>> GetAllTermineAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<BaseResponse<List<TerminDto>>>("termin");
                var termine = response.Daten.Select(t => TerminMapper.FromTerminDto(t)).ToList();
                return termine ?? throw new Exception("Laden der Termine in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Termine");
                throw new ServiceUnavailableException("Der Terminservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Get-Request für eine einzelnen Termin an die Haupt-API
        /// </summary>
        /// <param name="id">einzigartige Id des angeforderten Termins, die von MongoDB vergeben wird</param>
        /// <returns>Gewünschter Termin</returns>
        /// <exception cref="Exception">Fehler beim  Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<Termin> GetTerminByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Keine Id übermittelt.");
            }

            try
            {
                var terminDto = await _client.GetFromJsonAsync<TerminDto>($"termin/{id}");
                var termin = TerminMapper.FromTerminDto(terminDto);

                return termin ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Termins");
                throw new ServiceUnavailableException("Der Terminservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Delete Request an die Haupt-API
        /// </summary>
        /// <param name="id">Id des zu löschenden Termins</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Löschen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> DeleteTerminAsync(string id)
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
                var httpResponse = await _client.DeleteAsync($"termin/{id}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Löschen fehlgeschlagen für Termin-ID: {Id}, StatusCode: {StatusCode}", id, httpResponse.StatusCode);
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
                _logger.LogError(ex, "Fehler beim Löschen des Termins");
                throw new ServiceUnavailableException("Der Terminservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Löschen des Termins");
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
        /// Schickt einen Post-Request an die Haupt-API
        /// </summary>
        /// <param name="termin">Neu erstellter Termin</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Erstellen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> CreateTerminAsync(Termin termin)
        {
            try
            {
                var httpResponse = await _client.PostAsJsonAsync("termin", TerminMapper.ToCreateDto(termin));

                if (!httpResponse.IsSuccessStatusCode)
                {

                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Erstellen des Termins fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Termins");
                throw new ServiceUnavailableException("Der Terminservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Erstellen des Termins");
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
        /// <param name="termin">Aktualisierter Termin</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Aktualisieren der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> UpdateTerminAsync(Termin termin)
        {
            try
            {
                var httpResponse = await _client.PutAsJsonAsync($"termin/{termin.Id}", TerminMapper.ToUpdateDto(termin));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Aktualisieren fehlgeschlagen für Termin-ID: {Id}, StatusCode: {StatusCode}", termin.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Aktualisieren des Termins fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Termins");
                throw new ServiceUnavailableException("Der Terminservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Aktualisieren des Termins");
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
