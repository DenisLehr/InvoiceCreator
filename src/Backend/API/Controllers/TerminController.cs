using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Exceptions;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminController : ControllerBase
    {
        private readonly ILogger<TerminController> _logger;
        private readonly ITerminService _service;

        public TerminController(ILogger<TerminController> logger, ITerminService service)
        {
            _logger = logger;
            _service = service;
        }
        
        // GET: api/<TerminController>
        /// <summary>
        /// Lädt alle Termine
        /// </summary>
        /// <returns>Liste von Terminen</returns>
        /// <response code="200">Kunden erfolgreich geladen</response>
        /// <response code="404">Kunden nicht gefunden</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TerminDto>>> GetTermine()
        {
            try
            {
                var termine = await _service.GetTermineAsync();

                if (termine.Daten == null || !termine.Daten.Any())
                    return NotFound(new List<TerminDto>());

                return Ok(termine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Termine.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        /// <summary>
        /// Lädt eine paginierte und gefilterte Liste von Terminen.
        /// </summary>
        /// <param name="teileingabe">Suchbegriff</param>
        /// <param name="seite">Seitenzahl (beginnend bei 1)</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>Paginierte Terminliste</returns>
        /// <response code="200">Termine erfolgreich geladen</response>
        /// <response code="404">Keine Termine gefunden</response>
        [HttpGet("paged")]
        public async Task<ActionResult<PaginiertesResultDto<TerminDto>>> GetPagedTermine(string? teileingabe, int seite = 1, int eintraegeProSeite = 10)
        {

            if (seite < 1 || eintraegeProSeite < 1)
                return BadRequest(new { message = "Ungültige Paging-Parameter." });

            try
            {
                var result = await _service.GetPagedTermineAsync(teileingabe, seite, eintraegeProSeite);

                if (!result.DtoListe.Any())
                    return NotFound(new PaginiertesResultDto<TerminDto>
                    {
                        DtoListe = new List<TerminDto>(),
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Termine.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<TerminController>/5
        /// <summary>
        /// Lädt einen Termin
        /// </summary>
        /// <param name="id">ID des Termins</param>
        /// <returns>TerminDto</returns>
        /// <response code="200">Termin erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">Termin wurde nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<TerminDto>> GetTerminById(string id)
        {
            try
            {
                var termin = await _service.GetTerminByIdAsync(id);

                if (termin is null)
                    throw new NotFoundException("Termin konnte nicht gefunden werden.");

                return Ok(termin);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Keine Daten gefunden.");
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                var errorDetails = string.Join(";", ex.Errors);
                _logger.LogWarning(ex, "Ungültige Anfrage: {Message}", errorDetails);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Termins.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<TerminController>
        /// <summary>
        /// Erstellt einen Termin
        /// </summary>
        /// <param name="value">TerminDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Termin erfolgreich erstellt</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<bool>>> CreateTermin([FromBody] TerminDto value)
        {
            try
            {
                var result = await _service.CreateTerminAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen des Termins.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // POST api/<TerminController>
        /// <summary>
        /// Sendet eine Terminbestätigung mittels MailService an den Kunden
        /// </summary>
        /// <param name="value">EmailDto mit E-Mail Nachricht</param>
        /// <returns>BaseResponse mit Informationen über Erfolg inklusive Emailbestätigung</returns>
        /// <response code="200">Email erfolgreich versendet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost("termin/kunde")]
        public async Task<ActionResult<BaseResponse<bool>>> SendeTerminAnKunde([FromBody] TerminDto value)
        {
            try
            {
                var result = await _service.SendeTerminAnKundeAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Senden des Termins.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // POST api/<TerminController>
        /// <summary>
        /// Sendet eine Terminbestätigung mittels MailService an den Servicetechniker
        /// </summary>
        /// <param name="value">EmailDto mit E-Mail Nachricht</param>
        /// <returns>BaseResponse mit Informationen über Erfolg inklusive Emailbestätigung</returns>
        /// <response code="200">Email erfolgreich versendet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost("termin/techniker")]
        public async Task<ActionResult<BaseResponse<bool>>> SendeTerminAnServicetechniker([FromBody] TerminDto value)
        {
            try
            {
                var result = await _service.SendeTerminAnServicetechnikerAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Senden des Termins.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // PUT api/<TerminController>/5
        /// <summary>
        /// Update eines Termins
        /// </summary>
        /// <param name="id">übergebene Pfad-Id</param>
        /// <param name="value">TerminDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Kunde erfolgreich geupdatet</response>
        /// <response code="500">Unerwarteter Fehler</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateTermin(string id, [FromBody] TerminDto value)
        {
            if (id != value.Id)
                return BadRequest(new { message = "Pfad-ID stimmt nicht mit DTO-ID überein." });

            try
            {
                var result = await _service.UpdateTerminAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Updaten des Termins.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // DELETE api/<TerminController>/5
        /// <summary>
        /// Löscht einen Termin
        /// </summary>
        /// <param name="id">ID des Termins</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Termin erfolgreich gelöscht</response>
        /// <response code="500">Unerwarteter Fehler</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteTermin(string id)
        {
            try
            {
                var result = await _service.DeleteTerminAsync(id);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Termins.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }
    }
}
