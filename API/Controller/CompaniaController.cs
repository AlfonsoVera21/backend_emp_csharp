using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entidades;
using Infraestructura.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller;


[Route("api/[controller]")]
[ApiController]
public class CompaniaController:ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CompaniaController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Compania>>> GetCompanias(){
        var lista = await _db.Compania.ToListAsync();
        return Ok(lista); // status 200
    }
    [HttpGet("{id}", Name ="GetCompania")]
    public async Task<ActionResult<Compania>> GetCompania(int id){
        var comp = await _db.Compania.FindAsync(id);
        return Ok(comp); // status 200
    }
    [HttpPost]
    public async Task<ActionResult<Compania>> PostCompania([FromBody] Compania compania){
        await _db.Compania.AddAsync(compania);
        await _db.SaveChangesAsync();
        return CreatedAtRoute("GetCompania",new {id = compania.Id},compania); // status 201
    }
}
