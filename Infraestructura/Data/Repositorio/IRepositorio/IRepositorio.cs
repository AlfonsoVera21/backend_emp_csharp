using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Especificaciones;

namespace Infraestructura.Data.Repositorio.IRepositorio;

public interface IRepositorio<T> where T : class
{
    Task<IEnumerable<T>> ObtenerTodos(
        Expression<Func<T, bool>> filtro = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string incluirPropiedades = null //Compania,Cargo
    );
    Task<PagedList<T>> ObtenerTodosPaginado(
        Parametros parametros,
        Expression<Func<T, bool>> filtro = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string incluirPropiedades = null //Compania,Cargo
    );

    Task<T> ObtenerPrimero(
        Expression<Func<T, bool>> filtro = null,
        string incluirPropiedades = null 
    );
    Task Agregar(T entidad);

    void Revomer(T entidad);
}
