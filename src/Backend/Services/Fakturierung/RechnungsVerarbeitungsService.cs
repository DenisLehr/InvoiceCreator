using AutoMapper;
using Data.Interfaces;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Contracts.Interfaces;
using Shared.Contracts.Responses;
using Shared.Domain.ValueObjects;
using Shared.Dtos;
using Shared.Dtos.Email;
using Shared.Fakturierung;
using Shared.PdfGenerator;

namespace Services.Fakturierung
{
    public class RechnungsVerarbeitungsService : IRechnungsVerarbeitungsService
    {
        private readonly IFirmaRepository _firmaRepository;
        private readonly IKundeRepository _kundeRepository;
        private readonly IFakturierungService _fakturierungService;
        private readonly IPdfGeneratorService _pdfGenerator;
        private readonly IMapper _mapper;
        private readonly ILogger<RechnungsVerarbeitungsService> _logger;
        private readonly IEmailService _emailService;
        private readonly IRechnungRepository _rechnungRepository;

        public RechnungsVerarbeitungsService(IFirmaRepository firmaRepository, IKundeRepository kundeRepository, IFakturierungService fakturierungService, IPdfGeneratorService pdfGenerator, IMapper mapper, ILogger<RechnungsVerarbeitungsService> logger, IEmailService emailService, IRechnungRepository rechnungRepository)
        {
            _firmaRepository = firmaRepository;
            _kundeRepository = kundeRepository;
            _fakturierungService = fakturierungService;
            _pdfGenerator = pdfGenerator;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _rechnungRepository = rechnungRepository;
        }

        public async Task<BaseResponse<RechnungPdfResponseDto>> CreateRechnungAsync(CreateRechnungDto dto)
        {
            try
            {
                var kunde = await _kundeRepository.GetByIdAsync(dto.KundeId);
                if (kunde is null)
                {
                    _logger.LogWarning("Kunde mit ID {KundeId} nicht gefunden.", dto.KundeId);
                    return new BaseResponse<RechnungPdfResponseDto>
                    {
                        Erfolg = false,
                        Hinweis = "Rechnungserstellung fehlgeschlagen, Kunde konnte nicht gefunden werden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }


                var firma = await _firmaRepository.GetFirmendatenAsync();
                if (firma is null)
                {
                    _logger.LogWarning("Firmendaten konnten nicht geladen werden.");
                    return new BaseResponse<RechnungPdfResponseDto>
                    {
                        Erfolg = false,
                        Hinweis = "Rechnungserstellung fehlgeschlagen: Firmendaten nicht gefunden.",
                        Daten = null,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

                var rechnungsposten = dto.Rechnungsposten.Select(p => _mapper.Map<Rechnungsposten>(p)).ToList();

                var rechnung = _fakturierungService.ErstelleRechnung(rechnungsposten, firma, kunde, dto.UserInitial);

                var response = await _rechnungRepository.AddAsync(rechnung);

                if(!response)
                {
                    _logger.LogWarning("Rechnung konnte nicht gespeichert werden");
                }

                var rechnungDto = _mapper.Map<RechnungDto>(rechnung);
                

                var pdfBytes = _pdfGenerator.GeneriereRechnung(rechnung, kunde, firma);

                var filePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Rechnungen", "Rechnung_20251025.pdf");
                File.WriteAllBytesAsync(filePath, pdfBytes);

                return new BaseResponse<RechnungPdfResponseDto>
                {
                    Erfolg = true,
                    Hinweis = "Rechnung wurde erfolgreich erstellt.",
                    Daten = new RechnungPdfResponseDto
                    {
                        Rechnung = rechnungDto,
                        RechnungPdf = pdfBytes
                    },
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der Rechnungserstellung.");
                return new BaseResponse<RechnungPdfResponseDto>
                {
                    Erfolg = false,
                    Hinweis = "Ein technischer Fehler ist aufgetreten.",
                    Daten = null,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }

        public async Task<BaseResponse<bool>> SendeRechnungAnKunde(EmailDto email)
        {
            try
            {
                if (email == null)
                {
                    _logger.LogWarning("Keine E-Mail Daten übertragen.");
                    return new BaseResponse<bool>
                    {
                        Erfolg = false,
                        Hinweis = "Fehlende E-Mail Daten.",
                        Daten = false,
                        Zeitstempel = DateTime.UtcNow
                    };
                }

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
    }
}
