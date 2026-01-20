using InvoiceCreator_BlazorFrontend.Components.Common.Exceptions;
using InvoiceCreator_BlazorFrontend.Components.Userverwaltung.Mapper;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;

namespace InvoiceCreator_BlazorFrontend.Components.Userverwaltung.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly HttpClient _client;

        public UserService(ILogger<UserService> logger, HttpClient client) 
        {
            _logger = logger;
            _client = client;
        }

        /// <summary>
        /// Schickt einen Get-Request für alle User an die Haupt-API
        /// </summary>
        /// <returns>Liste aller User</returns>
        /// <exception cref="Exception">Fehler beim Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<BaseResponse<List<UserDto>>>("user");
                var users = response.Daten.Select(k => UserMapper.FromUserDto(k)).ToList();
                return users ?? throw new Exception("Laden der User in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der User");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Get-Request für eine einzelnen User an die Haupt-API
        /// </summary>
        /// <param name="id">einzigartige Id des angeforderten Users, die von MongoDB vergeben wird</param>
        /// <returns>Gewünschten User</returns>
        /// <exception cref="Exception">Fehler beim  Laden der Daten aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<User> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Keine Id übermittelt.");
            }

            try
            {
                var result = await _client.GetFromJsonAsync<BaseResponse<UserDto>>($"user/{id}");
                var user = UserMapper.FromUserDto(result.Daten);

                return user ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Users");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Delete Request an die Haupt-API
        /// </summary>
        /// <param name="id">Id des zu löschenden Users</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Löschen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client ist nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BaseResponse<bool> { Erfolg = false, Daten = false, Hinweis = "fehlende Id", Zeitstempel = DateTime.UtcNow };
            }

            try
            {
                var httpResponse = await _client.DeleteAsync($"user/{id}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Löschen fehlgeschlagen für User-ID: {Id}, StatusCode: {StatusCode}", id, httpResponse.StatusCode);
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
                _logger.LogError(ex, "Fehler beim Löschen des Users");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Löschen des Users");
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
        /// Lädt User je nach Auswahl der Einträge pro Seite und mit Suchfilter
        /// </summary>
        /// <param name="teileingabe">Suchbegriff für den Filter</param>
        /// <param name="seite">Gewünschte Seite</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>User und Informationen zur maximalen Anzahl aller Seiten für die Auswahlbuttons</returns>
        /// <exception cref="Exception">Fehler bei der Übertragung aus der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<PaginiertesResultDto<User>> GetPagedUsersAsync(string teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var query = $"user/paged?seite={seite}&eintraegeProSeite={eintraegeProSeite}";

                if (!string.IsNullOrWhiteSpace(teileingabe))
                    query += $"&teileingabe={Uri.EscapeDataString(teileingabe)}";

                var userDtos = await _client.GetFromJsonAsync<PaginiertesResultDto<UserDto>>(query);
                var user = userDtos.DtoListe.Select(k => UserMapper.FromUserDto(k)).ToList();
                var response = new PaginiertesResultDto<User>(user, userDtos.AnzahlSeiten, userDtos.ElementeProSeite, userDtos.AktuelleSeite);
                return response ?? throw new Exception("Laden der paginierten Userliste in API fehlgeschlagen");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der User");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
        }

        /// <summary>
        /// Schickt einen Post-Request an die Haupt-API
        /// </summary>
        /// <param name="user">Neu erstellter User</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Erstellen der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> CreateUserAsync(User user)
        {
            try
            {
                var httpResponse = await _client.PostAsJsonAsync("user", UserMapper.ToCreateDto(user));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erstellen fehlgeschlagen für User-ID: {Id}, StatusCode: {StatusCode}", user.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Erstellen des Users fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                  
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Users");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Erstellen des Users");
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
        /// <param name="user">Aktualisierter User</param>
        /// <returns>API Response mit Metadaten über Erfolg der Aktion, eventuelle Fehlermeldung und Zeitstempel</returns>
        /// <exception cref="Exception">Fehler beim Aktualisieren der Daten in der API</exception>
        /// <exception cref="ServiceUnavailableException">HTTP Client nicht erreichbar</exception>
        public async Task<BaseResponse<bool>> UpdateUserAsync(User user)
        {
            try
            {
                var httpResponse = await _client.PutAsJsonAsync($"kunde/{user.Id}", UserMapper.ToUpdateDto(user));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erstellen fehlgeschlagen für User-ID: {Id}, StatusCode: {StatusCode}", user.Id, httpResponse.StatusCode);
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Daten = false,
                        Hinweis = "Aktualisieren des Users fehlgeschlagen",
                        Zeitstempel = DateTime.UtcNow
                    };
                }
                  
                var response = await httpResponse.Content.ReadFromJsonAsync<BaseResponse<bool>>();

                return response ?? throw new Exception("Antwort konnte nicht gelesen werden.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Users");
                throw new ServiceUnavailableException("Der Userservice ist derzeit nicht erreichbar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Aktualisieren des Users");
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
