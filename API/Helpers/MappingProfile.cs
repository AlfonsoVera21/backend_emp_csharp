using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto;
using Core.Entidades;

namespace API.Helpers;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        CreateMap<Compania, CompaniaDto>().ReverseMap();

        CreateMap<Empleado, EmpleadoUpserDto>().ReverseMap();

        CreateMap<Empleado, EmpleadoReadDto>()
                    .ForMember(c => c.Compania, m => m.MapFrom(c => c.Compania.NombreCompania));
    }
}
