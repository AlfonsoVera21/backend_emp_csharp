using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Dto;
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
    private ResponseDto _response;
        private readonly ILogger<CompaniaController> _logger;
    public CompaniaController(ApplicationDbContext db, ILogger<CompaniaController> logger)
    {
        _logger = logger;
        _db = db;
        _response = new ResponseDto();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Compania>>> GetCompanias(){
        _logger.LogInformation("Listado de companias");
        var lista = await _db.Compania.ToListAsync();
        _response.Resultado = lista;
        _response.Mensaje = "Listado de Companias";
        return Ok(_response); // status 200
    }

    [HttpGet("{id}", Name ="GetCompania")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Compania>> GetCompania(int id){

        if(id==0){
            _logger.LogError("Debe de enviar el ID");
            _response.Mensaje="Debe de enviar el ID";
            _response.IsExitoso=false;
            return BadRequest(_response);
        }

        var comp = await _db.Compania.FindAsync(id);
        if(comp == null){
            _logger.LogError("La Compania no existe");
            _response.Mensaje="La Compania no existe";
            _response.IsExitoso=false;
            return NotFound(_response);
        }
        _logger.LogInformation("Datos de la compania");
        _response.Resultado = comp;
        _response.Mensaje = "Datos de la Compania"+ comp.Id;
        return Ok(_response); // status 200
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Compania>> PostCompania([FromBody] Compania compania){
        if(compania == null){
            _response.Mensaje = "Informacion Incorrecta";
            _response.IsExitoso=false;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        var companiaExiste = await _db.Compania.FirstOrDefaultAsync
                                (c => c.NombreCompania.ToLower()==compania.NombreCompania.ToLower());
        if(companiaExiste != null){
            ModelState.AddModelError("Nombre Duplicado","Nombre de la compania ya existe!");
            return BadRequest(ModelState);
        }

        await _db.Compania.AddAsync(compania);
        await _db.SaveChangesAsync();
        return CreatedAtRoute("GetCompania",new {id = compania.Id},compania); // status 201
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PutCompania(int id, [FromBody] Compania compania){
        if(id != compania.Id){
            return BadRequest("Id de compania no coincide");
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        
        var companiaExiste = await _db.Compania.FirstOrDefaultAsync
                            (c =>c.NombreCompania.ToLower() == compania.NombreCompania.ToLower()
                             && c.Id != compania.Id);
        if(companiaExiste != null){
            ModelState.AddModelError("NombreDuplicado","Nombre de la compania ya existe");
            return BadRequest(ModelState);
        }
        _db.Update(compania);
        await _db.SaveChangesAsync();
        return Ok(compania);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteCompania(int id){
        var compania = await _db.Compania.FindAsync(id);
        if(compania == null){
            return NotFound();
        }
        _db.Compania.Remove(compania);
        await _db.SaveChangesAsync();
        return NoContent();
    }


}
