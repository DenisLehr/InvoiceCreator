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
    public class LeistungService : ILeistungService
    {
        private readonly ILeistungRepository _repository;
        private readonly ILogger<LeistungService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<LeistungDto> _validator;

        public LeistungService(ILeistungRepository repository, ILogger<LeistungService> logger, IMapper mapper, IValidator<LeistungDto> validator)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse<List<LeistungDto>>> GetLeistungenAsync()
        {
            try
            {
                var leistungen = await _repository.GetAllAsync();

                var leistungenDto = leistungen.Select(k => _mapper.Map<LeistungDto>(k)).ToList();

                return new BaseResponse<List<LeistungDto>>
                {
                    Erfolg = true,
                    Hinweis = leistungenDto.Count > 0 ? "Leistungen erfolgreich geladen." : "Keine Leistungen vorhanden.",
                    Daten = leistungenDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Leistungen.");
                return new BaseResponse<List<LeistungDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<LeistungDto>> GetLeistungByIdAsync(string id)
        {
            try
            {
                var leistung = await _repository.GetByIdAsync(id);

                if (leistung == null)
                {
                    _logger.LogInformation("Keine Leistung mit der ID {Id} gefunden.", id);
                    return new BaseResponse<LeistungDto>
                    {
                        Erfolg = false,
                        Hinweis = "Leistung nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var leistungDto = _mapper.Map<LeistungDto>(leistung);

                return new BaseResponse<LeistungDto>
                {
                    Erfolg = true,
                    Hinweis = "Leistung erfolgreich geladen.",
                    Daten = leistungDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Leistung.");
                return new BaseResponse<LeistungDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<PaginiertesResultDto<LeistungDto>> GetPagedLeistungenAsync(string? teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var result = await _repository.GetPaginierteLeistungen(seite, eintraegeProSeite, teileingabe);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Leistungen.");
                return new PaginiertesResultDto<LeistungDto>
                {
                    DtoListe = new List<LeistungDto>(),
                    GesamtAnzahl = 1,
                    ElementeProSeite = eintraegeProSeite,
                    AktuelleSeite = seite
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateLeistungAsync(LeistungDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var leistung = _mapper.Map<Leistung>(dto);

                var letzterCode = await _repository.GetLetzterLeistungCodeAsync();
                var nextNumber = int.TryParse(letzterCode?.Substring(1), out var n) ? n + 1 : 1;
                var nextCode = $"S{nextNumber:D3}";

                leistung.Code = nextCode;

                var response = await _repository.AddAsync(leistung);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern der Leistung.");
                    throw new RepositoryException("Speichern der Leistung fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Leistung erfolgreich erstellt.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> UpdateLeistungAsync(LeistungDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var leistung = _mapper.Map<Leistung>(dto);

                var response = await _repository.UpdateAsync(leistung);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Aktualisieren der Leistung.");
                    throw new RepositoryException("Aktualisieren der Leistung fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Leistung erfolgreich aktualisiert.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Aktualisieren der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Aktualisieren der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Aktualisieren der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> DeleteLeistungAsync(string id)
        {
            try
            {
                var leistung = await _repository.GetByIdAsync(id);

                if (leistung == null)
                {
                    _logger.LogWarning("Leistung konnte nicht gefunden werden.");
                    throw new NotFoundException("Leistung konnte nicht gefunden werden.");
                }

                var result = await _repository.DeleteAsync(leistung);

                if (!result)
                {
                    _logger.LogWarning("Fehler beim Löschen der Leistung.");
                    throw new RepositoryException("Löschen der Leistung fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = result, Hinweis = "Leistung erfolgreich gelöscht", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Löschen der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen der Leistung.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
