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
    public class FirmendatenService : IFirmendatenService
    {
        private readonly IFirmaRepository _repository;
        private readonly ILogger<FirmendatenService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<FirmaDto> _validator;

        public FirmendatenService(IFirmaRepository repository, ILogger<FirmendatenService> logger, IMapper mapper, IValidator<FirmaDto> validator)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<FirmaDto> GetFirmendatenAsync()
        {
            try
            {
                var firmendaten = await _repository.GetFirmendatenAsync();

                if (firmendaten == null)
                {
                    throw new NotFoundException("Firmendaten konnten nicht gefunden werden.");
                }

                var firmendatenDto = _mapper.Map<FirmaDto>(firmendaten);
                return firmendatenDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Firmendaten.");
                throw;
            }
        }

        public async Task<BaseResponse<FirmaDto>> GetFirmaByIdAsync(string id)
        {
            try
            {
                var firma = await _repository.GetByIdAsync(id);

                if (firma == null)
                {
                    _logger.LogInformation("Keine Firma mit der ID {Id} gefunden.", id);
                    return new BaseResponse<FirmaDto>
                    {
                        Erfolg = false,
                        Hinweis = "Firma nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var firmaDto = _mapper.Map<FirmaDto>(firma);

                return new BaseResponse<FirmaDto>
                {
                    Erfolg = true,
                    Hinweis = "Firma erfolgreich geladen.",
                    Daten = firmaDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der Firma.");
                return new BaseResponse<FirmaDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateFirma(FirmaDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);
                
                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var firma = _mapper.Map<Firma>(dto);

                var response = await _repository.AddAsync(firma);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern der Firmendaten.");
                    throw new RepositoryException("Speichern der Firmendaten fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg= response , Hinweis = "Firmendaten erfolgreich erstellt.", Daten = false, Zeitstempel = DateTime.UtcNow};
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> UpdateFirma(FirmaDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var firma = _mapper.Map<Firma>(dto);

                var response = await _repository.UpdateAsync(firma);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Aktualisieren der Firmendaten.");
                    throw new RepositoryException("Aktualisieren der Firmendaten fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Firmendaten erfolgreich aktualisiert.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Aktualisieren der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Aktualisieren der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Aktualisieren der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> DeleteFirma(string id)
        {
            try
            {
                var firma = await _repository.GetByIdAsync(id);

                if (firma == null)
                {
                    _logger.LogWarning("Firmendaten konnten nicht gefunden werden.");
                    throw new NotFoundException("Firmendaten konnten nicht gefunden werden.");
                }

                var result = await _repository.DeleteAsync(firma);

                if (!result)
                {
                    _logger.LogWarning("Fehler beim Löschen der Firmendaten.");
                    throw new RepositoryException("Löschen der Firmendaten fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = result, Hinweis = "Firmendaten erfolgreich gelöscht", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Löschen der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen der Firmendaten.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
