using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entidades;
using Infraestructura.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    // llamo a las clases de configuracion creadas en la carpeta config
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ///base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CompaniaConfiguration());
        modelBuilder.ApplyConfiguration(new EmpleadoConfiguration());
    }



    public DbSet<Compania> Compania { get; set; }
    public DbSet<Empleado> Empleado { get; set; }
}
