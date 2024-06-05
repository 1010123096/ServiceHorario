using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Models;
using ServiceHorario.Servicios;
using System.Globalization;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioController : ControllerBase
    {
        private readonly HorarioService _horarioService;


        public HorarioController(HorarioService horarioService)
        {
            _horarioService = horarioService;

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateHorario(HorarioDto horariodto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState + "PRIMER FILTRO");
            }

            try
            {
                // Validación y parseo de la fecha y hora
                if (!DateTime.TryParseExact(horariodto.dia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                {
                    return BadRequest("El formato de la fecha es incorrecto. Use el formato 'yyyy-MM-dd'.");
                }


                var horarioExistente = await _horarioService.GetHorarioAsync(horariodto.IdMedico, fecha, horariodto.hora);

                if (horarioExistente != null)
                {
                    ModelState.AddModelError("horario", "Error al registrar el horario.");
                    return BadRequest(ModelState + "  TERCER FILTRO");
                }

                var horario = new Horario
                {
                    IdMedico = horariodto.IdMedico,
                    dia = fecha,
                    hora = horariodto.hora,

                };

                await _horarioService.CreateHorarioAsync(horario);

                // Aquí, en lugar de CreatedAtAction, puedes simplemente devolver el objeto creado
                return Ok(horario);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
           
        }
        [HttpGet("{idHorario}")]

        public async Task<IActionResult> GetHorario(string idHorario)
        {
            var horario = await _horarioService.GetHorarioIDAsync(idHorario);
            if (horario == null)
            {
                return NotFound();
            }

            return Ok(horario);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHorario()
        {
            var horarios = await _horarioService.GetHorariosAsync();
            return Ok(horarios);
        }
       
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> PutHorario(HorarioDto horariodto, String id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState + "PRIMER FILTRO");
            }

            try
            {
                // Validación y parseo de la fecha y hora
                if (!DateTime.TryParseExact(horariodto.dia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                {
                    return BadRequest("El formato de la fecha es incorrecto. Use el formato 'yyyy-MM-dd'.");
                }


                var horarioExistente = await _horarioService.GetHorarioIDAsync(id);
                if (!DateTime.TryParse(horariodto.dia, out DateTime dia))
                {
                    return BadRequest("El formato de fecha es incorrecto.");
                }

                if (horarioExistente == null)
                {
                    return NotFound("No se encontró el horario.");
                }



                horarioExistente.IdMedico = horariodto.IdMedico;
                   horarioExistente.dia = fecha;
    horarioExistente.hora = horariodto.hora;

                

                await _horarioService.ModificarHorarioAsync(horarioExistente);

                // Aquí, en lugar de CreatedAtAction, puedes simplemente devolver el objeto creado
                return Ok("Horario actualizada exitosamente.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarCita(string id)
        {
            try
            {
                // Obtener la cita existente
                var horarioExistente = await _horarioService.GetHorarioIDAsync(id);

                if (horarioExistente == null)
                {
                    return NotFound("No se encontró el horario.");
                }

    

                // Guardar la cita actualizada
                await _horarioService.EliminarHorarioAsync(horarioExistente);

                return Ok(" Horario eliminado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno500 Internal Server Error: {ex.Message}");
            }
        }
    } 
}
          
    

