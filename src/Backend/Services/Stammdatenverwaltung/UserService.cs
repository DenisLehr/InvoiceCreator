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
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<UserDto> _validator;

        public UserService(IUserRepository repository, ILogger<UserService> logger, IMapper mapper, IValidator<UserDto> validator)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BaseResponse<List<UserDto>>> GetUsersAsync()
        {
            try
            {
                var users = await _repository.GetAllAsync();

                var usersDto = users.Select(k => _mapper.Map<UserDto>(k)).ToList();

                return new BaseResponse<List<UserDto>>
                {
                    Erfolg = true,
                    Hinweis = usersDto.Count > 0 ? "User erfolgreich geladen." : "Keine User vorhanden.",
                    Daten = usersDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der User.");
                return new BaseResponse<List<UserDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<UserDto>> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogInformation("Keinen User mit der ID {Id} gefunden.", id);
                    return new BaseResponse<UserDto>
                    {
                        Erfolg = false,
                        Hinweis = "User nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var userDto = _mapper.Map<UserDto>(user);

                return new BaseResponse<UserDto>
                {
                    Erfolg = true,
                    Hinweis = "User erfolgreich geladen.",
                    Daten = userDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Users.");
                return new BaseResponse<UserDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<PaginiertesResultDto<UserDto>> GetPagedUsersAsync(string? teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var result = await _repository.GetPaginierteUser(seite, eintraegeProSeite, teileingabe);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten User.");
                return new PaginiertesResultDto<UserDto>
                {
                    DtoListe = new List<UserDto>(),
                    GesamtAnzahl = 1,
                    ElementeProSeite = eintraegeProSeite,
                    AktuelleSeite = seite
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateUserAsync(UserDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var user = _mapper.Map<User>(dto);

                var response = await _repository.AddAsync(user);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern des Users.");
                    throw new RepositoryException("Speichern des Users fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "User erfolgreich erstellt.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> UpdateUserAsync(UserDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var user = _mapper.Map<User>(dto);

                var response = await _repository.UpdateAsync(user);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Aktualisieren des Users.");
                    throw new RepositoryException("Aktualisieren des Users fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "User erfolgreich aktualisiert.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Aktualisieren des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Aktualisieren des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Aktualisieren des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User konnte nicht gefunden werden.");
                    throw new NotFoundException("User konnte nicht gefunden werden.");
                }

                var result = await _repository.DeleteAsync(user);

                if (!result)
                {
                    _logger.LogWarning("Fehler beim Löschen des Users.");
                    throw new RepositoryException("Löschen des Users fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = result, Hinweis = "User erfolgreich gelöscht", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Löschen des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Users.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
