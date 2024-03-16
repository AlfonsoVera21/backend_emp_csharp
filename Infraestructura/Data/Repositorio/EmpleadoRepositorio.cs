using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entidades;
using Infraestructura.Data.Repositorio.IRepositorio;

namespace Infraestructura.Data.Repositorio;

public class EmpleadoRepositorio : Repositorio<Empleado>, IEmpleadoRepositorio
{
    private readonly ApplicationDbContext _db;
    public EmpleadoRepositorio(ApplicationDbContext db):base(db)
    {
            _db = db;
    }
    public void Actualizar(Empleado empleado)
    {
        var empleadoDB = _db.Empleado.FirstOrDefault(e=>e.Id == empleado.Id);
        if(empleadoDB != null){
            empleadoDB.Nombres = empleado.Nombres;
            empleadoDB.Apellidos = empleado.Apellidos;
            empleadoDB.Cargo = empleado.Cargo;
            empleadoDB.CompaniaId = empleado.CompaniaId;
            _db.SaveChanges();
        }
    }
}
