using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto;
using Core.Entidades;
using Core.Especificaciones;
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
    private ResponsePaginadorDto _responsePaginador;
    private readonly ILogger<EmpleadoController> _logger;
    private readonly IMapper _mapper;
    public EmpleadoController(IUnidadTrabajo unidadTrabajo, ILogger<EmpleadoController> logger, IMapper mapper)
    {
        _unidadTrabajo = unidadTrabajo;
        _mapper = mapper;
        _logger = logger;
        _response = new ResponseDto();
        _responsePaginador = new ResponsePaginadorDto();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpleadoReadDto>>> GetEmpleados(
        [FromQuery] Parametros parametros
    ){
        _logger.LogInformation("Listado de Empleados");
        var lista = await _unidadTrabajo.Empleado.ObtenerTodosPaginado(
                                    parametros,
                                    incluirPropiedades:"Compania",
                                    orderBy:e=>e.OrderBy(e=>e.Apellidos)
                                                .ThenBy(e=>e.Nombres));
        // Para el uso de la Clase EmpleadosReadDto
        _responsePaginador.TotalPaginas = lista.Metadata.TotalPages;
        _responsePaginador.TotalRegistros = lista.Metadata.TotalCount;
        _responsePaginador.PageSize = lista.Metadata.PageSize;
        _responsePaginador.Resultado = _mapper.Map<IEnumerable<Empleado>,IEnumerable<EmpleadoReadDto>>(lista);
        _responsePaginador.Statuscode = HttpStatusCode.OK;
        _responsePaginador.Mensaje = "Listado de Empleados";
        return Ok(_responsePaginador); // status 200
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
             _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        var emp = await _unidadTrabajo.Empleado.ObtenerPrimero(e => e.Id == id, incluirPropiedades:"Compania");

        if(emp == null){
            _logger.LogError("Empleado no existe");
            _response.Mensaje="Empleado no existe";
            _response.IsExitoso=false;
             _response.Statuscode = HttpStatusCode.NotFound;
            return NotFound(_response);
        }
        _logger.LogInformation("Datos del Empleado");
        //Para el uso de la Clase EmpleadosReadDto
        _response.Resultado = _mapper.Map<Empleado, EmpleadoReadDto>(emp);
        _response.Mensaje = "Datos del Empleado"+ emp.Id;
        _response.Statuscode = HttpStatusCode.OK;
        return Ok(_response); // status 200
    }

    [HttpGet]
    [Route("EmpleadoPorCompania/{companiaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpleadoReadDto>>> GetEmpleadoPorCompania(int companiaId){
        _logger.LogInformation("Listado de Empleados por Compania");
        var lista = await _unidadTrabajo.Empleado.ObtenerTodos(e => e.CompaniaId==companiaId, incluirPropiedades:"Compania");
        if(lista.Count() == 0){
            _response.Mensaje = "No hay Listado de Empleados registrados en esta compania";
            _response.Statuscode = HttpStatusCode.OK;
            return Ok(_response);
        }
        _response.Resultado = _mapper.Map<IEnumerable<Empleado>,IEnumerable<EmpleadoReadDto>>(lista);
        _response.IsExitoso = true;
        _response.Mensaje = "Listado de Empleados por Compania";
        _response.Statuscode = HttpStatusCode.OK;
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
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            _response.Mensaje = "Informacion Incorrecta";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        var empleadoExiste = await _unidadTrabajo.Empleado.ObtenerPrimero(
                                        e => e.Nombres.ToLower() == empleadoDto.Nombres.ToLower() 
                                        && e.Apellidos.ToLower() == empleadoDto.Apellidos.ToLower());
        if(empleadoExiste != null){
            _response.Mensaje = "Nombre del empleado ya existe!";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        //uso del _mapper
        Empleado empleado = _mapper.Map<Empleado>(empleadoDto);

        await _unidadTrabajo.Empleado.Agregar(empleado);
        await _unidadTrabajo.Guardar();
        _response.Mensaje = "Empleado Creado con exito";
        _response.IsExitoso=true;
        _response.Statuscode = HttpStatusCode.Created;
        return CreatedAtRoute("GetEmpleados",new {id = empleado.Id},_response); // status 201
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PutEmpleado(int id, [FromBody] EmpleadoUpserDto empleadoDto){
        if(id != empleadoDto.Id){
            _response.Mensaje = "Id del empleado no coincide";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }

        if(!ModelState.IsValid){
            _response.Mensaje = "Id del empleado no coincide";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        
        var EmpleadoExiste = await _unidadTrabajo.Empleado.ObtenerPrimero(
                            c =>c.Apellidos.ToLower() == empleadoDto.Apellidos.ToLower()
                            && c.Nombres.ToLower() == empleadoDto.Nombres.ToLower()
                            && c.Id != empleadoDto.Id);
        if(EmpleadoExiste != null){
            _response.Mensaje = "Nombre del empleado ya existe!";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        Empleado empleado = _mapper.Map<Empleado>(empleadoDto);
        _unidadTrabajo.Empleado.Actualizar(empleado);
        await _unidadTrabajo.Guardar();
        _response.Mensaje = "Empleado actualizado correctamente";
        _response.IsExitoso=true;
        _response.Statuscode = HttpStatusCode.OK;
        return Ok(empleado);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEmpleado(int id){
        var empleado = await _unidadTrabajo.Empleado.ObtenerPrimero(e=>e.Id == id);
        if(empleado == null){
            _response.Mensaje = "Empleado no encontrado";
            _response.IsExitoso=false;
            _response.Statuscode = HttpStatusCode.NotFound;
            return NotFound(_response);
        }
        _unidadTrabajo.Empleado.Revomer(empleado);
        await _unidadTrabajo.Guardar();
        _response.Mensaje = "Empleado Eliminado con exito";
        _response.IsExitoso=true;
        _response.Statuscode = HttpStatusCode.NoContent;
        return Ok(_response);
    }


}
