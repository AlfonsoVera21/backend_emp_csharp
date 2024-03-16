using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infraestructura.Data.Repositorio.IRepositorio;

public interface IUnidadTrabajo : IDisposable
{
    ICompaniaRepositorio Compania {get; }
    IEmpleadoRepositorio Empleado {get; } 

    Task Guardar();
}
