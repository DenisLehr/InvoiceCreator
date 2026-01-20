using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace Services.Mapping
{
    public class KundeMapper: Profile
    {
        public KundeMapper() 
        {
            CreateMap<KundeDto, Kunde>();
            CreateMap<Kunde, KundeDto>();
            CreateMap<AdresseDto, Adresse>();
            CreateMap<Adresse, AdresseDto>();

            CreateMap<KundeDocument, Kunde>();
            CreateMap<Kunde, KundeDocument>();
            CreateMap<AdresseDocument, Adresse>();
            CreateMap<Adresse, AdresseDocument>();

            CreateMap<KundeDto, KundeDocument>();
            CreateMap<KundeDocument, KundeDto>();
            CreateMap<AdresseDto, AdresseDocument>();
            CreateMap<AdresseDocument, AdresseDto>();
        }
    }
}
