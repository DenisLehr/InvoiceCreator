using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Exceptions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RechnungController : ControllerBase
    {
        private readonly ILogger<RechnungController> _logger;
        private readonly IRechnungService _service;
        private readonly IRechnungsVerarbeitungsService _rechnungsVerarbeitungsService;

        public RechnungController(ILogger<RechnungController> logger, IRechnungService service, IRechnungsVerarbeitungsService rechnungsVerarbeitungsService)
        {
            _logger = logger;
            _service = service;
            _rechnungsVerarbeitungsService = rechnungsVerarbeitungsService;
        }

        // GET: api/<RechnungController>
        /// <summary>
        /// Lädt alle Rechnungen
        /// </summary>
        /// <returns>Liste von Rechnungen</returns>
        /// <response code="200">Rechnungen erfolgreich geladen</response>
        /// <response code="404">Rechnungen nicht gefunden</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RechnungDto>>> GetRechnungen()
        {
            try
            {
                var rechnungen = await _service.GetRechnungenAsync();

                if (rechnungen.Daten == null || !rechnungen.Daten.Any())
                    return NotFound(new List<RechnungDto>());

                return Ok(rechnungen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Rechnungen.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<RechnungController>/5
        /// <summary>
        /// Lädt eine Rechnung 
        /// </summary>
        /// <param name="id">ID der Rechnung</param>
        /// <returns>Rechnung</returns>
        /// <response code="200">Rechnung erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">Rechnung wurde nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<RechnungDto>> GetRechnungById(string id)
        {
            try
            {
                var rechnung = await _service.GetRechnungByIdAsync(id);

                if (rechnung is null)
                    throw new NotFoundException("Rechnung konnte nicht gefunden werden.");

                return Ok(rechnung);
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Rechnung.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<RechnungController>
        /// <summary>
        /// Speichert die lokal erstellte Rechnung in der Datenbank
        /// </summary>
        /// <param name="value">RechnungDto</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Rechnung erfolgreich gespeichert</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost("save")]
        public async Task<ActionResult<BaseResponse<bool>>> SpeicherRechnungOnline([FromBody] RechnungDto value)
        {
            try
            {
                var result = await _service.SpeicherRechnungAsync(value);
                
                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Rechnung.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // POST api/<RechnungController>
        /// <summary>
        /// Erstellt eine Rechnung im Backend
        /// </summary>
        /// <param name="value">Id des Kunden, UserInitialien, Liste von Rechnungsposten</param>
        /// <returns>BaseResponse mit Rechnungsdaten inklusive Rechnungs-Pdf als Byte-Array</returns>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponse<RechnungPdfResponseDto>>> CreateRechnung([FromBody] CreateRechnungDto value)
        {
            try
            {
                var result = await _rechnungsVerarbeitungsService.CreateRechnungAsync(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Rechnungserstellung fehlgeschlagen: {Hinweis}", result.Hinweis);
                    return BadRequest(new BaseResponse<object>
                    {
                        Erfolg = false,
                        Hinweis = result.Hinweis,
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    });
                }


                var speicherResult = await _service.SpeicherRechnungAsync(result.Daten.Rechnung);

                if (!speicherResult.Erfolg)
                {
                    _logger.LogWarning("Rechnungsspeicherung nach erfolgreicher Erstellung fehlgeschlagen: {Hinweis}", speicherResult.Hinweis);
                }

                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                var errorDetails = ex.Errors != null ? string.Join(";", ex.Errors) : "Keine Details verfügbar";
                _logger.LogWarning(ex, "Ungültige Anfrage: {Details}", errorDetails);

                return BadRequest(new BaseResponse<object>
                {
                    Erfolg = false,
                    Hinweis = ex.Message,
                    Daten = ex.Errors,
                    Zeitstempel = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen der Rechnung.");

                return StatusCode(500, new BaseResponse<object>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                });
            }
        }

        // PUT api/<RechnungController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RechnungController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
