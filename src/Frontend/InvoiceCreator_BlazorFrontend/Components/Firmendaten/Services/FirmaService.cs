using InvoiceCreator_BlazorFrontend.Components.Common.Exceptions;
using InvoiceCreator_BlazorFrontend.Components.Firmendaten.Mapper;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Firmendaten.Services
{
    public class FirmaService
    {
        private readonly HttpClient _client;
        private readonly ILogger<FirmaService> _logger;

        public FirmaService(HttpClient client, ILogger<FirmaService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Schickt einen Get-Request für alle Firmen an die Haupt-API
        /// </summary>
        /// <returns>Liste aller Firmen</returns>
        /// <exception cref="Exception">Fehler beim Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<Firma> GetFirmendatenAsync()
        {
            try
            {
                var firmaDto = await _client.GetFromJsonAsync<FirmaDto>("firma");
                var firma = FirmaMapper.FromFirmaDto(firmaDto);
                return firma ?? throw new Exception("Laden der Firmendaten in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Firmen");
                throw new ServiceUnavailableException("Der Firmaservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Get-Request für eine einzelne Firma an die Haupt-API
        /// </summary>
        /// <param name="id">einzigartige Id der angeforderten Firma, die von MongoDB vergeben wird</param>
        /// <returns>Gewünschte Firma</returns>
        /// <exception cref="Exception">Fehler beim  Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<Firma> GetFirmaByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Keine Id übermittelt.");
            }

            try
            {
                var firmaDto = await _client.GetFromJsonAsync<FirmaDto>($"firma/{id}");
                var firma = FirmaMapper.FromFirmaDto(firmaDto);
                //if (firma.Name.Contains("Ciblu"))
                //{
                //    firma.LogoUrl = MediaPaths.KleinesLogoVonFirma();
                //}

                return firma ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Firma");
                throw new ServiceUnavailableException("Der Firmaservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Delete Request an die Haupt-API
        /// </summary>
        /// <param name="id">Id der zu löschenden Firma</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Löschen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> DeleteFirmaAsync(string id)
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
                var httpResponse = await _client.DeleteAsync($"firma/{id}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Löschen fehlgeschlagen für Firma-ID: {Id}, StatusCode: {StatusCode}", id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Löschen der Firma fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                    

                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen der Firma");
                throw new ServiceUnavailableException("Der Firmaservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Löschen der Firma");
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
        /// <param name="firma">Neu erstellte Firma</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Erstellen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> CreateFirmaAsync(Firma firma)
        {
            try
            {
                var httpResponse = await _client.PostAsJsonAsync("firma", FirmaMapper.ToCreateDto(firma));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erstellen fehlgeschlagen für Firma-ID: {Id}, StatusCode: {StatusCode}", firma.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Erstellen der Firma fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                  
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Firma");
                throw new ServiceUnavailableException("Der Firmaservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Erstellen der Firma");
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
        /// <param name="firma">Aktualisierte Firma</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Aktualisieren der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> UpdateFirmaAsync(Firma firma)
        {
            try
            {
                var httpResponse = await _client.PutAsJsonAsync($"firma/{firma.Id}", FirmaMapper.ToUpdateDto(firma));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Aktualisieren fehlgeschlagen für Firma-ID: {Id}, StatusCode: {StatusCode}", firma.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Aktualisieren der Firma fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                   
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der Firma");
                throw new ServiceUnavailableException("Der Firmaservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Aktualisieren der Firma");
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
