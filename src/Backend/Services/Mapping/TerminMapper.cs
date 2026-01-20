using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Models;
using Shared.Dtos;

namespace Services.Mapping
{
    public class TerminMapper: Profile
    {
        public TerminMapper() 
        { 
            CreateMap<Termin, TerminDto>();
            CreateMap<TerminDto, Termin>();

            CreateMap<Termin, TerminDocument>();
            CreateMap<TerminDocument, Termin>();

            CreateMap<TerminDto, TerminDocument>();
            CreateMap<TerminDocument, TerminDto>();
        }
    }
}
