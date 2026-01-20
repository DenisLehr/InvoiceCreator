using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace Services.Mapping
{
    public class FirmaMapper: Profile
    {
        public FirmaMapper() 
        {
            CreateMap<Firma, FirmaDto>();
            CreateMap<FirmaDto, Firma>();
            CreateMap<BankverbindungDto, Bankverbindung>();
            CreateMap<Bankverbindung, BankverbindungDto>();

            CreateMap<FirmaDocument, Firma>();
            CreateMap<Firma, FirmaDocument>();
            CreateMap<Bankverbindung, BankverbindungDocument>();
            CreateMap<BankverbindungDocument, Bankverbindung>();

            CreateMap<FirmaDocument, FirmaDto>();
            CreateMap<FirmaDto, FirmaDocument>();
            CreateMap<BankverbindungDto, BankverbindungDocument>();
            CreateMap<BankverbindungDocument, BankverbindungDto>();
            
        }
    }
}
