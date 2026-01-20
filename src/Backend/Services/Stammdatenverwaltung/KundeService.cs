using AutoMapper;
using Data.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Contracts.Responses;
using Shared.Domain.Models;
using Shared.Dtos;
using Shared.Dtos.Email;
using Shared.Exceptions;

namespace Services.Stammdatenverwaltung
{
    public class KundeService : IKundeService
    {
        private readonly IKundeRepository _repository;
        private readonly ILogger<KundeService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<KundeDto> _validator;

        public KundeService(IKundeRepository repository, ILogger<KundeService> logger, IMapper mapper, IValidator<KundeDto> validator)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse<List<KundeDto>>> GetKundenAsync()
        {
            try
            {
                var kunden = await _repository.GetAllAsync();

                var kundenDto = kunden.Select(k => _mapper.Map<KundeDto>(k)).ToList();

                return new BaseResponse<List<KundeDto>>
                {
                    Erfolg = true,
                    Hinweis = kundenDto.Count > 0 ? "Kunden erfolgreich geladen." : "Keine Kunden vorhanden.",
                    Daten = kundenDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Kunden.");
                return new BaseResponse<List<KundeDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<KundeDto>> GetKundeByIdAsync(string id)
        {
            try
            {
                var kunde = await _repository.GetByIdAsync(id);

                if (kunde == null)
                {
                    _logger.LogInformation("Kein Kunde mit der ID {Id} gefunden.", id);
                    return new BaseResponse<KundeDto>
                    {
                        Erfolg = false,
                        Hinweis = "Kunde nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var kundeDto = _mapper.Map<KundeDto>(kunde);

                return new BaseResponse<KundeDto>
                {
                    Erfolg = true,
                    Hinweis = "Kunde erfolgreich geladen.",
                    Daten = kundeDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Kunden.");
                return new BaseResponse<KundeDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<List<KundeDto>>> GetKundenByNameAsync(string name)
        {
            try
            {
                var kundenDto = await _repository.GetKundenNamenAsync(name);

                return new BaseResponse<List<KundeDto>>
                {
                    Erfolg = true,
                    Hinweis = kundenDto.Count > 0 ? "Kunden erfolgreich geladen." : "Keine Kunden vorhanden.",
                    Daten = kundenDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Kunden.");
                return new BaseResponse<List<KundeDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<PaginiertesResultDto<KundeDto>> GetPagedKundenAsync(string? teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var result = await _repository.GetPaginierteKunden(seite, eintraegeProSeite, teileingabe);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Kunden.");
                return new PaginiertesResultDto<KundeDto>
                {
                    DtoListe = new List<KundeDto>(),
                    GesamtAnzahl = 1,
                    ElementeProSeite = eintraegeProSeite,
                    AktuelleSeite = seite
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateKundeAsync(KundeDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var kunde = _mapper.Map<Kunde>(dto);

                var response = await _repository.AddAsync(kunde);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern des Kunden.");
                    throw new RepositoryException("Speichern des Kunden fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Kunde erfolgreich erstellt.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
        
        public async Task<BaseResponse<bool>> SendeEmailAnKundeAsync(SendeEmailRequestDto emailRequestDto)
        {
            return new BaseResponse<bool>
            {
                Erfolg = false,
                Hinweis = "",
                Daten = false,
                Zeitstempel = DateTime.UtcNow
            };
        }

        public async Task<BaseResponse<bool>> UpdateKundeAsync(KundeDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var kunde = _mapper.Map<Kunde>(dto);

                var response = await _repository.UpdateAsync(kunde);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Aktualisieren des Kunden.");
                    throw new RepositoryException("Aktualisieren des Kunden fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Kunde erfolgreich aktualisiert.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Aktualisieren des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Aktualisieren des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Aktualisieren des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> DeleteKundeAsync(string id)
        {
            try
            {
                var kunde = await _repository.GetByIdAsync(id);

                if (kunde == null)
                {
                    _logger.LogWarning("Kunde konnte nicht gefunden werden.");
                    throw new NotFoundException("Kunde konnte nicht gefunden werden.");
                }

                var result = await _repository.DeleteAsync(kunde);

                if (!result)
                {
                    _logger.LogWarning("Fehler beim Löschen des Kunden.");
                    throw new RepositoryException("Löschen des Kunden fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = result, Hinweis = "Kunde erfolgreich gelöscht", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Löschen des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Kunden.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
