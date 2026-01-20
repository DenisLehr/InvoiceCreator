using AutoMapper;
using Data.Persistence.Documents;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.Dtos;

namespace Services.Mapping
{
    public class RechnungMapper: Profile
    {
        public RechnungMapper() 
        {
            CreateMap<Rechnungsposten, RechnungspostenDto>();
            CreateMap<RechnungspostenDto, Rechnungsposten>();
            CreateMap<Rechnung, RechnungDto>();
            CreateMap<RechnungDto, Rechnung>();

            CreateMap<Rechnungsposten, RechnungspostenDocument>();
            CreateMap<RechnungspostenDocument, Rechnungsposten>();
            CreateMap<Rechnung, RechnungDocument>();
            CreateMap<RechnungDocument, Rechnung>();

            CreateMap<RechnungspostenDto, RechnungspostenDocument>();
            CreateMap<RechnungspostenDocument, RechnungspostenDto>();
            

            CreateMap<RechnungDocument, RechnungDto>();
            CreateMap<RechnungDto, RechnungDocument>();

            // Enum Mappings
            //CreateMap<SteuersatzDto, Steuersatz>().ConvertUsing(src => (Steuersatz)src);
            //CreateMap<Steuersatz, SteuersatzDto>().ConvertUsing(src => (SteuersatzDto)src);
            //CreateMap<WaehrungDto, Waehrung>().ConvertUsing(src => (Waehrung)src);
            //CreateMap<Waehrung, WaehrungDto>().ConvertUsing(src => (WaehrungDto)src);
            CreateMap<decimal, Steuersatz>().ConvertUsing(src => (Steuersatz)(int)src);
            CreateMap<Steuersatz, decimal>().ConvertUsing(src => (decimal)src);

            CreateMap<int, Steuersatz>().ConvertUsing(src => (Steuersatz)src);
            CreateMap<Steuersatz, int>().ConvertUsing(src => (int)src);

        }
    }
}
