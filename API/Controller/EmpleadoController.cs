using System;
using System.Collections.Generic;
using System.Linq;
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
public class EmpleadoController:ControllerBase
{
    private readonly IUnidadTrabajo _unidadTrabajo;
    private ResponseDto _response;
    private readonly ILogger<EmpleadoController> _logger;
    private readonly IMapper _mapper;
    public EmpleadoController(IUnidadTrabajo unidadTrabajo, ILogger<EmpleadoController> logger, IMapper mapper)
    {
        _unidadTrabajo = unidadTrabajo;
        _mapper = mapper;
        _logger = logger;
        _response = new ResponseDto();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpleadoReadDto>>> GetEmpleados(){
        _logger.LogInformation("Listado de Empleados");
        var lista = await _unidadTrabajo.Empleado.ObtenerTodos(incluirPropiedades:"Compania");
        // Para el uso de la Clase EmpleadosReadDto
        _response.Resultado = _mapper.Map<IEnumerable<Empleado>,IEnumerable<EmpleadoReadDto>>(lista);
        _response.Mensaje = "Listado de Empleados";
        return Ok(_response); // status 200
    }

    [HttpGet("{id}", Name ="GetEmpleados")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Empleado>> GetEmpleados(int id){

        if(id==0){
            _logger.LogError("Debe de enviar el ID");
            _response.Mensaje="Debe de enviar el ID";
            _response.IsExitoso=false;
            return BadRequest(_response);
        }

        var emp = await _unidadTrabajo.Empleado.ObtenerPrimero(e => e.Id == id, incluirPropiedades:"Compania");

        if(emp == null){
            _logger.LogError("Empleado no existe");
            _response.Mensaje="Empleado no existe";
            _response.IsExitoso=false;
            return NotFound(_response);
        }
        _logger.LogInformation("Datos del Empleado");
        //Para el uso de la Clase EmpleadosReadDto
        _response.Resultado = _mapper.Map<Empleado, EmpleadoReadDto>(emp);
        _response.Mensaje = "Datos del Empleado"+ emp.Id;
        return Ok(_response); // status 200
    }

    [HttpGet]
    [Route("EmpleadoPorCompania/{companiaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpleadoReadDto>>> GetEmpleadoPorCompania(int companiaId){
        _logger.LogInformation("Listado de Empleados por Compania");
        var lista = await _unidadTrabajo.Empleado.ObtenerTodos(e => e.CompaniaId==companiaId, incluirPropiedades:"Compania");
        _response.Resultado = _mapper.Map<IEnumerable<Empleado>,IEnumerable<EmpleadoReadDto>>(lista);
        _response.IsExitoso = true;
        _response.Mensaje = "Listado de Empleados por Compania";
        return Ok(_response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Empleado>> PostEmpleado([FromBody] EmpleadoUpserDto empleadoDto){
        if(empleadoDto == null){
            _response.Mensaje = "Informacion Incorrecta";
            _response.IsExitoso=false;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        var empleadoExiste = await _unidadTrabajo.Empleado.ObtenerPrimero(
                                        e => e.Nombres.ToLower() == empleadoDto.Nombres.ToLower() 
                                        && e.Apellidos.ToLower() == empleadoDto.Apellidos.ToLower());
        if(empleadoExiste != null){
            ModelState.AddModelError("Nombre Duplicado","Nombre del empleado ya existe!");
            return BadRequest(ModelState);
        }
        //uso del _mapper
        Empleado empleado = _mapper.Map<Empleado>(empleadoDto);

        await _unidadTrabajo.Empleado.Agregar(empleado);
        await _unidadTrabajo.Guardar();
        return CreatedAtRoute("GetEmpleados",new {id = empleado.Id},empleado); // status 201
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PutEmpleado(int id, [FromBody] EmpleadoUpserDto empleadoDto){
        if(id != empleadoDto.Id){
            return BadRequest("Id del empleado no coincide");
        }

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        
        var EmpleadoExiste = await _unidadTrabajo.Empleado.ObtenerPrimero(
                            c =>c.Apellidos.ToLower() == empleadoDto.Apellidos.ToLower()
                            && c.Nombres.ToLower() == empleadoDto.Nombres.ToLower()
                            && c.Id != empleadoDto.Id);
        if(EmpleadoExiste != null){
            ModelState.AddModelError("NombreDuplicado","Nombre del empleado ya existe");
            return BadRequest(ModelState);
        }
        Empleado empleado = _mapper.Map<Empleado>(empleadoDto);
        _unidadTrabajo.Empleado.Actualizar(empleado);
        await _unidadTrabajo.Guardar();
        return Ok(empleado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEmpleado(int id){
        var empleado = await _unidadTrabajo.Empleado.ObtenerPrimero(e=>e.Id == id);
        if(empleado == null){
            return NotFound();
        }
        _unidadTrabajo.Empleado.Revomer(empleado);
        await _unidadTrabajo.Guardar();
        return NoContent();
    }


}
