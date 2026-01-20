using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Dtos.Email;
using Shared.Exceptions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KundeController : ControllerBase
    {
        private readonly ILogger<KundeController> _logger;
        private readonly IKundeService _service;

        public KundeController(ILogger<KundeController> logger,IKundeService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/<KundeController>
        /// <summary>
        /// Lädt alle Kunden
        /// </summary>
        /// <returns>Liste von Kunden</returns>
        /// <response code="200">Kunden erfolgreich geladen</response>
        /// <response code="404">Kunden nicht gefunden</response>
        [HttpGet]
        public async Task<ActionResult<BaseResponse<IEnumerable<KundeDto>>>> GetKunden()
        {
            try
            {
                var kunden = await _service.GetKundenAsync();

                if (kunden.Daten == null || !kunden.Daten.Any())
                    return NotFound(new List<KundeDto>());

                return Ok(kunden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Kunden.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        /// <summary>
        /// Lädt eine paginierte und gefilterte Liste von Kunden.
        /// </summary>
        /// <param name="teileingabe">Suchbegriff</param>
        /// <param name="seite">Seitenzahl (beginnend bei 1)</param>
        /// <param name="eintraegeProSeite">Anzahl der Einträge pro Seite</param>
        /// <returns>Paginierte Kundenliste</returns>
        /// <response code="200">Kunden erfolgreich geladen</response>
        /// <response code="404">Keine Kunden gefunden</response>
        [HttpGet("paged")]
        public async Task<ActionResult<PaginiertesResultDto<KundeDto>>> GetPagedKunden(string? teileingabe, int seite = 1, int eintraegeProSeite = 10)
        {

            if (seite < 1 || eintraegeProSeite < 1)
                return BadRequest(new { message = "Ungültige Paging-Parameter." });

            try
            {
                var result = await _service.GetPagedKundenAsync(teileingabe, seite, eintraegeProSeite);

                if (!result.DtoListe.Any())
                    return NotFound(new PaginiertesResultDto<KundeDto>
                    {
                        DtoListe = new List<KundeDto>(),
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Kunden.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<KundeController>/5
        /// <summary>
        /// Lädt einen Kunden
        /// </summary>
        /// <param name="id">ID des Kunden</param>
        /// <returns>Kunde</returns>
        /// <response code="200">Kunde erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">Kunde wurde nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<KundeDto>>> GetKundeById(string id)
        {
            try
            {
                var kunde = await _service.GetKundeByIdAsync(id);

                if (kunde is null)
                    throw new NotFoundException("Kunde konnte nicht gefunden werden.");

                return Ok(kunde);
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Kunden.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<KundeController>/5
        /// <summary>
        /// Lädt eine Kundenliste von maximal 10 Kunden je nach gesuchtem Eingabestring
        /// </summary>
        /// <param name="name">Teil des Kundennamen</param>
        /// <returns>Kundenliste mit gesuchtem Namen</returns>
        /// <response code="200">Kunden erfolgreich geladen</response>
        /// <response code="404">Kunden wurde nicht gefunden</response>
        [HttpGet("search/{name}")]
        public async Task<ActionResult<BaseResponse<List<KundeDto>>>> SucheKundenNamen(string name)
        {
            try
            {
                var kunden = await _service.GetKundenByNameAsync(name);

                if (!kunden.Daten.Any())
                    throw new NotFoundException("Keine Kunden gefunden.");

                return Ok(kunden);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Keine Daten gefunden.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Kunden.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<KundeController>
        /// <summary>
        /// Erstellt einen Kunden
        /// </summary>
        /// <param name="value">KundeDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Kunde erfolgreich erstellt</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<bool>>> CreateKunde([FromBody] KundeDto value)
        {
            try
            {
                var result = await _service.CreateKundeAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen des Kunden.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // POST api/<KundeController>
        /// <summary>
        /// Sendet eine E-Mail mittels MailService an den Kunden
        /// </summary>
        /// <param name="value">EmailDto mit E-Mail Nachricht</param>
        /// <returns>BaseResponse mit Informationen über Erfolg inklusive Emailbestätigung</returns>
        /// <response code="200">Email erfolgreich versendet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost("sendEmail")]
        public async Task<ActionResult<BaseResponse<bool>>> SendeEmailAnKunde([FromBody] SendeEmailRequestDto value)
        {
            try
            {
                var result = await _service.SendeEmailAnKundeAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Senden der Rechnung.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }


        // PUT api/<KundeController>/5
        /// <summary>
        /// Update eines Kunden
        /// </summary>
        /// <param name="id">übergebene Pfad-Id</param>
        /// <param name="value">KundeDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Kunde erfolgreich geupdatet</response>
        /// <response code="500">Unerwarteter Fehler</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateKunde(string id,[FromBody] KundeDto value)
        {
            if (id != value.Id)
                return BadRequest(new { message = "Pfad-ID stimmt nicht mit DTO-ID überein." });

            try
            {
                var result = await _service.UpdateKundeAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Updaten des Kunden.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // DELETE api/<KundeController>/5
        /// <summary>
        /// Löscht einen Kunden
        /// </summary>
        /// <param name="id">ID des Kunden</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Kunde erfolgreich gelöscht</response>
        /// <response code="500">Unerwarteter Fehler</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteKunde(string id)
        {
            try
            {
                var result = await _service.DeleteKundeAsync(id);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Kunden.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }
    }
}
