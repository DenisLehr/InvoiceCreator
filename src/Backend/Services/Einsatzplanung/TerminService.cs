using AutoMapper;
using Data.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Contracts.Interfaces;
using Shared.Contracts.Responses;
using Shared.Domain.Enums;
using Shared.Domain.Extensions;
using Shared.Domain.Models;
using Shared.Dtos;
using Shared.Dtos.Email;
using Shared.Dtos.Enums;
using Shared.Exceptions;

namespace Services.Einsatzplanung
{
    public class TerminService : ITerminService
    {
        private readonly ITerminRepository _repository;
        private readonly ILogger<TerminService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<TerminDto> _validator;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IKundeRepository _kundeRepository;
        private readonly IEmailMusterProvider _emailMuster;
        private readonly IFirmaRepository _firmaRepository;

        public TerminService(ITerminRepository repository, ILogger<TerminService> logger, IMapper mapper, IValidator<TerminDto> validator,IEmailService emailService, IUserRepository userRepository, IKundeRepository kundeRepository, IEmailMusterProvider emailMuster, IFirmaRepository firmaRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
            _emailService = emailService;
            _userRepository = userRepository;
            _kundeRepository = kundeRepository;
            _emailMuster = emailMuster;
            _firmaRepository = firmaRepository;
        }

        public async Task<BaseResponse<List<TerminDto>>> GetTermineAsync()
        {
            try
            {
                var termine = await _repository.GetAllAsync();

                var termineDto = termine.Select(k => _mapper.Map<TerminDto>(k)).ToList();

                return new BaseResponse<List<TerminDto>>
                {
                    Erfolg = true,
                    Hinweis = termineDto.Count > 0 ? "Termine erfolgreich geladen." : "Keine Termin vorhanden.",
                    Daten = termineDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Termins.");
                return new BaseResponse<List<TerminDto>>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = [],
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<TerminDto>> GetTerminByIdAsync(string id)
        {
            try
            {
                var termin = await _repository.GetByIdAsync(id);

                if (termin == null)
                {
                    _logger.LogInformation("Kein Termin mit der ID {Id} gefunden.", id);
                    return new BaseResponse<TerminDto>
                    {
                        Erfolg = false,
                        Hinweis = "Termin nicht vorhanden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var terminDto = _mapper.Map<TerminDto>(termin);

                return new BaseResponse<TerminDto>
                {
                    Erfolg = true,
                    Hinweis = "Termin erfolgreich geladen.",
                    Daten = terminDto,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden des Termins.");
                return new BaseResponse<TerminDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<PaginiertesResultDto<TerminDto>> GetPagedTermineAsync(string? teileingabe, int seite, int eintraegeProSeite)
        {
            try
            {
                var result = await _repository.GetPaginierteTermine(seite, eintraegeProSeite, teileingabe);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Laden der paginierten Termine.");
                return new PaginiertesResultDto<TerminDto>
                {
                    DtoListe = new List<TerminDto>(),
                    GesamtAnzahl = 1,
                    ElementeProSeite = eintraegeProSeite,
                    AktuelleSeite = seite
                };
            }
        }

        public async Task<BaseResponse<bool>> CreateTerminAsync(TerminDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var termin = _mapper.Map<Termin>(dto);

                var kunde = await _kundeRepository.GetByIdAsync(termin.KundeId);

                if (kunde is null)
                {
                    _logger.LogWarning("Termin konnte nicht beim Kunden hinterlegt werden.");
                }
                else
                {
                    kunde.Termine.Add(termin);
                    var updateResponse = await _kundeRepository.UpdateAsync(kunde);

                    if (!updateResponse)
                        _logger.LogWarning("Kunde konnte nicht aktualisiert werden.");
                }

                var response = await _repository.AddAsync(termin);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Speichern des Termins.");
                    throw new RepositoryException("Speichern des Termins fehlgeschlagen.");
                }

                var emailKundeResponse = await SendeTerminAnKundeAsync(dto);
                var emailServiceMitarbeiterResponse = await SendeTerminAnServicetechnikerAsync(dto);

                var hinweis = "Termin erfolgreich erstellt.";
                if (!emailKundeResponse.Erfolg)
                    hinweis += $" Hinweis: E-Mail an Kunden fehlgeschlagen ({emailKundeResponse.Hinweis}).";

                if (!emailServiceMitarbeiterResponse.Erfolg)
                    hinweis += $" Hinweis: E-Mail an Servicetechniker fehlgeschlagen ({emailServiceMitarbeiterResponse.Hinweis}).";

                return new BaseResponse<bool> 
                {
                    Erfolg = true, 
                    Hinweis = hinweis, 
                    Daten = false, 
                    Zeitstempel = DateTime.UtcNow 
                };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Speichern des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Speichern des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> SendeTerminAnKundeAsync(TerminDto termin)
        {
            if (termin == null)
            {
                _logger.LogWarning("Keine Termin-Daten übertragen.");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Hinweis = "Fehlende Termin-Daten.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }

            try
            {
                var firma = await _firmaRepository.GetFirmendatenAsync();
                var kunde = await _kundeRepository.GetByIdAsync(termin.KundeId);

                if (firma == null || kunde == null)
                {
                    _logger.LogWarning("Firmendaten oder Kunde nicht gefunden.");
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Hinweis = "Firmendaten oder Kunde nicht vorhanden.",
                        Daten = false,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var emailPlatzhalter = new Dictionary<string, string>
                {
                    ["Name"] = kunde.Geschlecht switch
                    {
                        Geschlecht.maennlich => $"Herr {kunde.Nachname}",
                        Geschlecht.weiblich => $"Frau {kunde.Nachname}",
                        _ => $"{kunde.Vorname} {kunde.Nachname}"
                    },
                    ["Datum"] = termin.Start.ToString("dd.MM.yyyy"),
                    ["Uhrzeit"] = termin.Start.ToString("HH:mm"),
                    ["Ansprechpartner"] = firma.Kontaktperson,
                    ["Firmenname"] = firma.Name
                };

                var email = new EmailDto
                {
                    EmpfaengerEmail = kunde.Email,
                    Betreff = EmailMusterTyp.TerminBestaetigungKunde.GetDisplayName(),
                    Nachricht = _emailMuster.GetMuster(EmailMusterTyp.TerminBestaetigungKunde, emailPlatzhalter)
                };
                
                var result = await _emailService.SendeNachrichtAsync(email);

                if (!result.Erfolg)
                {
                    _logger.LogWarning("E-Mail-Versand fehlgeschlagen.");
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Hinweis = "E-Mail konnte nicht versendet werden.",
                        Daten = false,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                return new BaseResponse<bool>
                {
                    Erfolg = true,
                    Hinweis = "E-Mail wurde erfolgreich versendet.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "E-Mail-Versand fehlgeschlagen.");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Hinweis = "Fehler beim E-Mail-Versand aufgetreten.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<bool>> SendeTerminAnServicetechnikerAsync(TerminDto termin)
        {
            if (termin == null)
            {
                _logger.LogWarning("Keine Termin-Daten übertragen.");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Hinweis = "Fehlende Termin-Daten.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            
            try
            {
                string leistungen = String.Join("\n", termin.Leistungen);
                var servicetechniker = await _userRepository.GetByIdAsync(termin.UserId);
                var kunde = await _kundeRepository.GetByIdAsync(termin.KundeId);

                if (servicetechniker == null || kunde == null)
                {
                    _logger.LogWarning("Servicetechniker oder Kunde nicht gefunden.");
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Hinweis = "Servicetechniker oder Kunde nicht vorhanden.",
                        Daten = false,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                leistungen = String.Join(",\n", leistungen);

                var emailPlatzhalter = new Dictionary<string, string>
                {
                    ["Datum"] = termin.Start.ToString("dd.MM.yyyy"),
                    ["Uhrzeit"] = termin.Start.ToString("HH:mm"),
                    ["Name"] = kunde.Geschlecht switch 
                    {
                        Geschlecht.maennlich => $"Herrn {kunde.Nachname}",
                        Geschlecht.weiblich => $"Frau {kunde.Nachname}",
                        _ => $"{kunde.Nachname}"
                    },
                    ["Strasse"] = kunde.Adresse.Strasse,
                    ["Hausnummer"] = kunde.Adresse.Hausnummer,
                    ["Hausnummerzusatz"] = kunde.Adresse.Hausnummerzusatz != null? kunde.Adresse.Hausnummerzusatz: "",
                    ["PLZ"] = kunde.Adresse.PLZ,
                    ["Stadt"] = kunde.Adresse.Stadt,
                    ["Leistungen"] = leistungen,
                    ["Dauer"] = $"{termin.GeschätzteDauer.TotalMinutes} Min",
                    ["Notiz"] = termin.Text
                };

                var email = new EmailDto
                {
                    EmpfaengerEmail = servicetechniker.Email,
                    Betreff = EmailMusterTyp.TerminBestaetigungServicetechniker.GetDisplayName(),
                    Nachricht = _emailMuster.GetMuster(EmailMusterTyp.TerminBestaetigungServicetechniker, emailPlatzhalter)
                };
                var result = await _emailService.SendeNachrichtAsync(email);

                if (result == null)
                {
                    _logger.LogWarning("E-Mail-Versand fehlgeschlagen.");
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Hinweis = "E-Mail konnte nicht versendet werden.",
                        Daten = false,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                return new BaseResponse<bool>
                {
                    Erfolg = true,
                    Hinweis = "E-Mail wurde erfolgreich versendet.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "E-Mail-Versand fehlgeschlagen.");
                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Hinweis = "Fehler beim E-Mail-Versand aufgetreten.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateTerminAsync(TerminDto dto)
        {
            try
            {
                var result = await _validator.ValidateAsync(dto);

                if (!result.IsValid)
                    throw new Shared.Exceptions.ValidationException(result.Errors.Select(e => e.ErrorMessage).ToArray());

                var termin = _mapper.Map<Termin>(dto);

                var response = await _repository.UpdateAsync(termin);

                if (!response)
                {
                    _logger.LogWarning("Fehler beim Aktualisieren des Termins.");
                    throw new RepositoryException("Aktualisieren des Termins fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = response, Hinweis = "Termin erfolgreich aktualisiert.", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Shared.Exceptions.ValidationException ex)
            {
                _logger.LogError(ex, "Validierungs-Fehler beim Aktualisieren des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Aktualisieren des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Aktualisieren des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }

        public async Task<BaseResponse<bool>> DeleteTerminAsync(string id)
        {
            try
            {
                var termin = await _repository.GetByIdAsync(id);

                if (termin == null)
                {
                    _logger.LogWarning("Termin konnte nicht gefunden werden.");
                    throw new NotFoundException("Termin konnte nicht gefunden werden.");
                }

                var result = await _repository.DeleteAsync(termin);

                if (!result)
                {
                    _logger.LogWarning("Fehler beim Löschen des Termins.");
                    throw new RepositoryException("Löschen des Termins fehlgeschlagen.");
                }

                return new BaseResponse<bool> { Erfolg = result, Hinweis = "Termin erfolgreich gelöscht", Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository-Fehler beim Löschen des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Löschen des Termins.");
                return new BaseResponse<bool> { Erfolg = false, Hinweis = ex.Message, Daten = false, Zeitstempel = DateTime.UtcNow };
            }
        }
    }
}
