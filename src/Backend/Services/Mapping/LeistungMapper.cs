using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace Services.Mapping
{
    public class LeistungMapper: Profile
    {
        public LeistungMapper() 
        {
            CreateMap<LeistungDto, Leistung>();
            CreateMap<Leistung, LeistungDto>();
            CreateMap<ZusatzlogikDto, Zusatzlogik>();
            CreateMap<Zusatzlogik, ZusatzlogikDto>();

            CreateMap<LeistungDocument, Leistung>();
            CreateMap<Leistung, LeistungDocument>();
            CreateMap<ZusatzlogikDocument, Zusatzlogik>();
            CreateMap<Zusatzlogik, ZusatzlogikDocument>();

            CreateMap<LeistungDto, LeistungDocument>();
            CreateMap<LeistungDocument, LeistungDto>();
            CreateMap<ZusatzlogikDto, ZusatzlogikDocument>();
            CreateMap<ZusatzlogikDocument, ZusatzlogikDto>();
        }
    }
}
