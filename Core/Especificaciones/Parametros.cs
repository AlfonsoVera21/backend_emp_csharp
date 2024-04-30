using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Especificaciones;

public class Parametros
{
    private const int MaxPageSize = 50;
    public int PagesNumber { get; set; } = 1;
    private int _PageSize = 10;
    public int PageSize {
        get =>_PageSize;
        set => _PageSize = (value > MaxPageSize)? MaxPageSize:value;
    }


}
