using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos;
using Shared.Exceptions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmaController : ControllerBase
    {
        private readonly ILogger<FirmaController> _logger;
        private readonly IFirmendatenService _service;

        public FirmaController(ILogger<FirmaController> logger, IFirmendatenService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: api/<FirmaController>
        /// <summary>
        /// Lädt die Default Firmendaten
        /// </summary>
        /// <returns>FirmaDto</returns>
        /// <response code="200">Firmendaten erfolgreich geladen</response>
        /// <response code="404">Firmendaten nicht gefunden</response>
        /// <response code="500">Unerwarteter Fehler</response>
        [HttpGet]
        public async Task<ActionResult<FirmaDto>> GetFirmendaten()
        {
            try
            {
                var firmendaten = await _service.GetFirmendatenAsync();

                if (firmendaten is null)
                    throw new NotFoundException("Keine Firmendaten gefunden.");

                return Ok(firmendaten);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Keine Firmendaten gefunden.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Firmendaten.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // GET api/<FirmaController>/5
        /// <summary>
        /// Lädt eine Firma anhand der Id
        /// </summary>
        /// <param name="id">ID der Firma</param>
        /// <returns>Kunde</returns>
        /// <response code="200">Firma erfolgreich geladen</response>
        /// <response code="400">Ungültige Eingabe</response>
        /// <response code="404">Firma wurde nicht gefunden</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<FirmaDto>>> GetFirmaById(string id)
        {
            try
            {
                var firma = await _service.GetFirmaByIdAsync(id);

                if (firma is null)
                    throw new NotFoundException("Firma konnte nicht gefunden werden.");

                return Ok(firma);
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
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Firma.");
                return StatusCode(500, new { message = "Ein technischer Fehler ist aufgetreten." });
            }
        }

        // POST api/<FirmaController>
        /// <summary>
        /// Erstellt eine Firma
        /// </summary>
        /// <param name="value">FirmaDto ohne Id</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Firma erfolgreich erstellt</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<bool>>> CreateFirma([FromBody] FirmaDto value)
        {
            try
            {
                var result = await _service.CreateFirma(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Erstellen der Firmendaten.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }

        }

        // PUT api/<FirmaController>/5
        /// <summary>
        /// Updatet eine Firma
        /// </summary>
        /// <param name="id">übergebene Pfad-Id</param>
        /// <param name="value">FirmaDto mit Id</param>
        /// <returns>BaseResponse mit Informationen über Erfolg</returns>
        /// <response code="200">Firma erfolgreich geupdatet</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateFirma(string id, [FromBody] FirmaDto value)
        {
            if (id != value.Id)
                return BadRequest(new { message = "Pfad-ID stimmt nicht mit DTO-ID überein." });

            try
            {
                var result = await _service.UpdateFirma(value);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Updaten der Firmendaten.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }

        // DELETE api/<FirmaController>/5
        /// <summary>
        /// Löscht eine Firma
        /// </summary>
        /// <param name="id">Id der Firma</param>
        /// <returns>BaseResponse mit Information über Erfolg</returns>
        /// <response code="200">Firma erfolgreich gelöscht</response>
        /// <response code="400">Ungültige Eingabe</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteFirma(string id)
        {
            try
            {
                var result = await _service.DeleteFirma(id);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("Ungültige Anfrage: {Hinweis}", result.Hinweis);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen der Firmendaten.");
                return StatusCode(500, new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow });
            }
        }
    }
}
