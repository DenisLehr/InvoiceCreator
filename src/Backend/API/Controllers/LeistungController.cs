using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Exceptions;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeistungController : ControllerBase
    {
        private readonly ILogger<LeistungController> _logger;
        private readonly ILeistungService _service;

        public LeistungController(ILogger<LeistungController> logger, ILeistungService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/<LeistungController>
        /// <summary>
        /// Lädt den Leistungskatalog
        /// </summary>
        /// <returns>Liste von Leistungen</returns>
        /// <response code="200">Leistungskatalog erfolgreich geladen</response>
        /// <response code="404">Leistungskatalog nicht gefunden</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeistungDto>>> GetLeistungen()
        {
            try
            {
                var leistungen = await _service.GetLeistungenAsync();

                if (leistungen.Daten == null || !leistungen.Daten.Any())
                    return NotFound(new List<LeistungDto>());

                return Ok(leistungen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Leistungen.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<LeistungController>/5
        /// <summary>
        /// Lädt eine Leistung aus dem Leistungskatalog
        /// </summary>
        /// <param name="id">ID der Leistung</param>
        /// <returns>Leistung</returns>
        /// <response code="200">Leistung erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">Leistung wurde nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeistungDto>> GetLeistungById(string id)
        {
            try
            {
                var leistung = await _service.GetLeistungByIdAsync(id);

                if (leistung is null)
                    throw new NotFoundException("Leistung konnte nicht gefunden werden.");

                return Ok(leistung);
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Leistung.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        /// <summary>
        /// Lädt eine paginierte und gefilterte Liste von Leistungen.
        /// </summary>
        /// <param name="teileingabe">Suchbegriff</param>
        /// <param name="seite">Seitenzahl (beginnend bei 1)</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>Paginierte Leistungenliste</returns>
        /// <response code="200">Leistungen erfolgreich geladen</response>
        /// <response code="404">Keine Leistungen gefunden</response>
        [HttpGet("paged")]
        public async Task<ActionResult<PaginiertesResultDto<LeistungDto>>> GetPagedLeistungen(string? teileingabe, int seite = 1, int eintraegeProSeite = 10)
        {
            if (seite < 1 || eintraegeProSeite < 1)
                return BadRequest(new { message = "Ungültige Paging-Parameter." });

            try
            {
                var result = await _service.GetPagedLeistungenAsync(teileingabe, seite, eintraegeProSeite);

                if (!result.DtoListe.Any())
                    return NotFound(new PaginiertesResultDto<LeistungDto>
                    {
                        DtoListe = new List<LeistungDto>(),
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Leistungen.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<LeistungController>
        /// <summary>
        /// Erstellt eine Leistung
        /// </summary>
        /// <param name="value">LeistungDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Leistung erfolgreich erstellt</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<bool>>> CreateLeistung([FromBody] LeistungDto value)
        {
            try
            {
                var result = await _service.CreateLeistungAsync(value);
                
                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen der Leistung.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // PUT api/<UserController>/5
        /// <summary>
        /// Update einer Leistung
        /// </summary>
        /// <param name="id">übergebene Pfad-Id</param>
        /// <param name="value">LeistungDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Leistung erfolgreich geupdatet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateLeistung(string id, [FromBody] LeistungDto value)
        {
            if (id != value.Id)
                return BadRequest(new { message = "Pfad-ID stimmt nicht mit DTO-ID überein." });

            try
            {
                var result = await _service.UpdateLeistungAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Updaten der Leistung.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // DELETE api/<UserController>/5
        /// <summary>
        /// Löscht eine Leistung
        /// </summary>
        /// <param name="id">ID der Leistung</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Leistung erfolgreich gelöscht</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteLeistung(string id)
        {
            try
            {
                var result = await _service.DeleteLeistungAsync(id);
                
                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen der Leistung.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }
    }
}
