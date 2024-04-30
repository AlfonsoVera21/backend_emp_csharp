using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Especificaciones;

public class PagedList<T> : List<T>
{
    public Metadata Metadata { get; set; }

    public PagedList(List<T> items, int count, int pageNumber,int pageSize)
    {
        Metadata = new Metadata{
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize) //1.5 lo converte en 2
        };   
        AddRange(items);     
    }
    public static PagedList<T> ToPagedList(IEnumerable<T> entidad,int pageNumber, int pageSize){
        var count = entidad.Count();
        var items = entidad.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items,count,pageNumber,pageSize);
    }
}
