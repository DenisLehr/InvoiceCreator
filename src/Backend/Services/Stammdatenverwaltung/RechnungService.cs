using AutoMapper;
using Data.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;
using Shared.Exceptions;

namespace Services.Stammdatenverwaltung
{
    public class RechnungService : IRechnungService
    {
        private readonly IRechnungRepository _repository;
        private readonly ILogger<RechnungService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<RechnungDto> _validator;

        public RechnungService(IRechnungRepository repository, ILogger<RechnungService> logger, IMapper mapper, IValidator<RechnungDto> validator)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse<List<RechnungDto>>> GetRechnungenAsync()
        {
            try
            {
                var rechnungen = await _repository.GetAllAsync();

                var rechnungenDto = rechnungen.Select(k => _mapper.Map<RechnungDto>(k)).ToList();

                return new BaseResponse<List<RechnungDto>>
                {
                    Erfolg = true,
                    Hinweis = rechnungenDto.Count > 0 ? "Rechnungen erfolgreich geladen." : "Keine Rechnungen vorhanden.",
                    Daten = rechnungenDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Rechnungen.");
                return new BaseResponse<List<RechnungDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<RechnungDto>> GetRechnungByIdAsync(string id)
        {
            try
            {
                var rechnung = await _repository.GetByIdAsync(id);

                if (rechnung == null)
                {
                    _logger.LogInformation("Keine Rechnung mit der ID {Id} gefunden.", id);
                    return new BaseResponse<RechnungDto>
                    {
                        Erfolg = false,
                        Hinweis = "Rechnung nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var rechnungDto = _mapper.Map<RechnungDto>(rechnung);

                return new BaseResponse<RechnungDto>
                {
                    Erfolg = true,
                    Hinweis = "Rechnung erfolgreich geladen.",
                    Daten = rechnungDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Rechnung.");
                return new BaseResponse<RechnungDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<RechnungDto>> GetRechnungByRechnungsnummerAsync(string rechnungsnummer)
        {
            try
            {
                var rechnung = await _repository.GetRechnungMitRechnungsNummerAsync(rechnungsnummer);

                if (rechnung == null)
                {
                    _logger.LogInformation("Keine Rechnung mit der Rechnungsnummer {Rechnungsnummer} gefunden.", rechnungsnummer);
                    return new BaseResponse<RechnungDto>
                    {
                        Erfolg = false,
                        Hinweis = "Rechnung nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var rechnungDto = _mapper.Map<RechnungDto>(rechnung);

                return new BaseResponse<RechnungDto>
                {
                    Erfolg = true,
                    Hinweis = "Rechnung erfolgreich geladen.",
                    Daten = rechnungDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Rechnung.");
                return new BaseResponse<RechnungDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<PaginiertesResultDto<RechnungDto>> GetPagedRechnungenAsync(string? teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var result = await _repository.GetPaginierteRechnungen(seite, eintraegeProSeite, teileingabe);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Rechnungen.");
                return new PaginiertesResultDto<RechnungDto>
                {
                    DtoListe = new List<RechnungDto>(),
                    GesamtAnzahl = 1,
                    ElementeProSeite = eintraegeProSeite,
                    AktuelleSeite = seite
                };
            }
        }

        public async Task<BaseResponse<bool>> SpeicherRechnungAsync(RechnungDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var rechnung = _mapper.Map<Rechnung>(dto);

                var response = await _repository.AddAsync(rechnung);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern der Rechnung.");
                    throw new RepositoryException("Speichern der Rechnung fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Rechnung erfolgreich erstellt.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern der Rechnung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern der Rechnung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Rechnung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
