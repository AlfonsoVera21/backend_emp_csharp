using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto;
using Core.Entidades;
using Infraestructura.Data;
using Infraestructura.Data.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller;


[Route("api/[controller]")]
[ApiController]
public class CompaniaController:ControllerBase
{
    private ResponseDto _response;
    private ResponsePaginadorDto _responsePaginador;
    private readonly ILogger<CompaniaController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnidadTrabajo _unidadTrabajo;
    public CompaniaController(IUnidadTrabajo unidadTrabajo, ILogger<CompaniaController> logger, IMapper mapper)
    {
        _unidadTrabajo = unidadTrabajo;
        _mapper = mapper;
        _logger = logger;
        _response = new ResponseDto();
        _responsePaginador = new ResponsePaginadorDto();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Compania>>> GetCompanias(){
        _logger.LogInformation("Listado de companias");
        var lista = await _unidadTrabajo.Compania.ObtenerTodos();
        _response.Resultado = lista;
        _response.Mensaje = "Listado de Companias";
        _response.Statuscode = HttpStatusCode.OK;
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
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        var comp = await _unidadTrabajo.Compania.ObtenerPrimero(c=>c.Id == id);
        if(comp == null){
            _logger.LogError("La Compania no existe");
            _response.Mensaje="La Compania no existe";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.NotFound;
            return NotFound(_response);
        }
        _logger.LogInformation("Datos de la compania");
        _response.Resultado = comp;
        _response.Mensaje = "Datos de la Compania"+ comp.Id;
        _response.IsExitoso = true;
        _response.Statuscode = HttpStatusCode.OK;
        return Ok(_response); // status 200
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Compania>> PostCompania([FromBody] CompaniaDto companiaDto){
        if(companiaDto == null){
            _response.Mensaje = "Informacion Incorrecta";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        var companiaExiste = await _unidadTrabajo.Compania.ObtenerPrimero
            (c => c.NombreCompania.ToLower()==companiaDto.NombreCompania.ToLower());
        if(companiaExiste != null){
            //ModelState.AddModelError("Nombre Duplicado","Nombre de la compania ya existe!");
            _response.IsExitoso = false;
            _response.Mensaje = "Nombre de la compania ya existe";
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        //uso del _mapper
        Compania compania = _mapper.Map<Compania>(companiaDto);

        await _unidadTrabajo.Compania.Agregar(compania);
        await _unidadTrabajo.Guardar();
        _response.IsExitoso = true;
        _response.Mensaje = "Compania creada con exito";
        _response.Statuscode = HttpStatusCode.Created;
        return CreatedAtRoute("GetCompania",new {id = compania.Id},_response); // status 201
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PutCompania(int id, [FromBody] CompaniaDto companiaDto){
        if(id != companiaDto.Id){
            _response.IsExitoso = false;
            _response.Mensaje = "Id de compania no coincide";
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        
        var companiaExiste = await _unidadTrabajo.Compania.ObtenerPrimero(c =>c.NombreCompania.ToLower() == companiaDto.NombreCompania.ToLower()
                             && c.Id != companiaDto.Id);
        if(companiaExiste != null){
            _response.IsExitoso = false;
            _response.Mensaje = "Nombre de la compania ya existe";
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        Compania compania = _mapper.Map<Compania>(companiaDto);
        _unidadTrabajo.Compania.Actualizar(compania);
        await _unidadTrabajo.Guardar();
        _response.IsExitoso = true;
        _response.Mensaje = "Compania actualizada";
        _response.Statuscode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteCompania(int id){
        var compania = await _unidadTrabajo.Compania.ObtenerPrimero(c => c.Id == id);

        if(compania == null){
            _response.IsExitoso=false;
            _response.Mensaje= "Compania no encontrada";
            _response.Statuscode = HttpStatusCode.NotFound;
            return NotFound(_response);
        }
        _unidadTrabajo.Compania.Revomer(compania);
        await _unidadTrabajo.Guardar();
        _response.IsExitoso = true;
        _response.Mensaje = "Compania eliminada";
        _response.Statuscode = HttpStatusCode.NoContent;
        return Ok(_response);
    }


}
