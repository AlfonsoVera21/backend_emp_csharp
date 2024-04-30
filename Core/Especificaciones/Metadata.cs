using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Especificaciones;

public class Metadata
{
    public int CurrentPage { get; set; } //pagina actual
    public int TotalPages { get; set; } // total de paginas
    public int PageSize { get; set; } //tamano de la pagina
    public int TotalCount { get; set; } // total registro
    public bool HasPrevius => CurrentPage > 1; //paginas previas
    public bool HasNext => CurrentPage < TotalPages; //pagina siguiente
}
