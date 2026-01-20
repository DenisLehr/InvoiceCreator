using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Exceptions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _service;

        public UserController(ILogger<UserController> logger, IUserService service) 
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/<UserController>
        /// <summary>
        /// Lädt eine Liste alle User
        /// </summary>
        /// <returns>Liste von Usern</returns>
        /// <response code="200">Userliste erfolgreich geladen</response>
        /// <response code="404">User nicht gefunden</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _service.GetUsersAsync();

                if (users.Daten == null || !users.Daten.Any())
                    return NotFound(new List<UserDto>());

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Userliste.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<UserController>/5
        /// <summary>
        /// Lädt einen User mit der übergebenen ID
        /// </summary>
        /// <param name="id">ID des Users</param>
        /// <returns>Gesuchten User</returns>
        /// <response code="200">User erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">User nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        { 
            try
            {
                var user = await _service.GetUserByIdAsync(id);

                if (user is null)
                    throw new NotFoundException("User konnte nicht gefunden werden.");

                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Keine Daten gefunden.");
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                var errorDetails = string.Join(";", ex.Errors);
                _logger.LogWarning(ex, "Ungültige Anfrage: {Message}",errorDetails);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Users.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        /// <summary>
        /// Lädt eine paginierte und gefilterte Liste von Usern.
        /// </summary>
        /// <param name="teileingabe">Suchbegriff</param>
        /// <param name="seite">Seitenzahl (beginnend bei 1)</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>Paginierte Userliste</returns>
        /// <response code="200">User erfolgreich geladen</response>
        /// <response code="404">Keine User gefunden</response>
        [HttpGet("paged")]
        public async Task<ActionResult<PaginiertesResultDto<UserDto>>> GetPagedUsers(string? teileingabe, int seite = 1, int eintraegeProSeite = 10)
        {
            if (seite < 1 || eintraegeProSeite < 1)
                return BadRequest(new { message = "Ungültige Paging-Parameter." });

            try
            {
                var result = await _service.GetPagedUsersAsync(teileingabe, seite, eintraegeProSeite);

                if (!result.DtoListe.Any())
                    return NotFound(new PaginiertesResultDto<UserDto>
                    {
                        DtoListe = new List<UserDto>(),
                        ElementeProSeite = eintraegeProSeite,
                        AktuelleSeite = seite,
                        GesamtAnzahl = 1
                    });

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Keine Daten gefunden.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten User.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<UserController>
        /// <summary>
        /// Erstellt einen User
        /// </summary>
        /// <param name="value">UserDto ohne Id</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">User erfolgreich erstellt</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="400">User existiert schon</response>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<bool>>> CreateUser([FromBody] UserDto value)
        {
            try
            {
                var result = await _service.CreateUserAsync(value);
                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UserAlreadyExistsException ex)
            {
                return BadRequest(new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen des Users.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }

        }

        // PUT api/<UserController>/5
        /// <summary>
        /// Updatet einen User
        /// </summary>
        /// <param name="id">übergebene Pfad-Id</param>
        /// <param name="value">UserDto mit Id</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">User erfolgreich geupdatet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateUser(string id, [FromBody] UserDto value)
        {
            if (id != value.Id)
                return BadRequest(new { message = "Pfad-ID stimmt nicht mit DTO-ID überein." });

            try
            {
                var result = await _service.UpdateUserAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Updaten des Benutzers.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // DELETE api/<UserController>/5
        /// <summary>
        /// Löscht einen User
        /// </summary>
        /// <param name="id">Id des Users</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">User erfolgreich gelöscht</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteUser(string id)
        {
            try
            {
                var result = await _service.DeleteUserAsync(id);
                
                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Users.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }
    }
}
