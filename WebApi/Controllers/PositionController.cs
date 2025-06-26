using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.DTOs;
using WebApi.DTOs.Filters;
using WebApi.Services.Commands;
using WebApi.Services.Queries;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionQueryService _queryService;
        private readonly IPositionCommandService _commandService;

        public PositionsController(
            IPositionQueryService queryService,
            IPositionCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// Obtiene todas las posiciones existentes.
        /// </summary>
        /// <returns>Lista de posiciones</returns>
        /// <response code="200">Lista obtenida correctamente</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Obtiene todas las posiciones existentes", OperationId = "GetAllPositions")]
        [ProducesResponseType(typeof(IEnumerable<PositionDTO>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _queryService.GetAllAsync();
            return Ok(positions);
        }

        /// <summary>
        /// Obtiene una posición por su ID.
        /// </summary>
        /// <param name="id">ID de la posición</param>
        /// <returns>Detalles de la posición</returns>
        /// <response code="200">Posición encontrada</response>
        /// <response code="404">No existe la posición</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpGet("Get/{id}")]
        [SwaggerOperation(Summary = "Obtiene la posición a la que pertenece el id que viene por parámetro", OperationId = "GetPositionById")]
        [ProducesResponseType(typeof(PositionDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetById(int id)
        {
            var position = await _queryService.GetByIdAsync(id);
            if (position == null) return NotFound();
            return Ok(position);
        }

        /// <summary>
        /// Crea una nueva posición.
        /// </summary>
        /// <param name="dto">Datos de la posición</param>
        /// <returns>Posición creada</returns>
        /// <response code="201">Creado exitosamente</response>
        /// <response code="400">Error de validación</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Inserta una nueva posición", OperationId = "AddPosition")]
        [ProducesResponseType(typeof(PositionDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Create(PositionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _commandService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una posición existente.
        /// </summary>
        /// <param name="id">ID de la posición</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Sin contenido si fue exitoso</returns>
        /// <response code="204">Actualizado correctamente</response>
        /// <response code="404">No se encontró la posición</response>
        /// <response code="400">Error de validación</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpPut("Update/{id}")]
        [SwaggerOperation(Summary = "Actualiza la posición a la cual pertenece el id que viene por parámetro", OperationId = "UpdatePosition")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Update(int id, PositionDTO dto)
        {
            var updated = await _commandService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Elimina una posición por su ID.
        /// </summary>
        /// <param name="id">ID de la posición</param>
        /// <returns>Sin contenido si fue exitoso</returns>
        /// <response code="204">Eliminado correctamente</response>
        /// <response code="404">No se encontró la posición</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Elimina la posición a la cual pertenece el id que viene por parámetro", OperationId = "DeletePosition")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _commandService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Obtiene una lista de posiciones aplicando filtros opcionales como estado, ubicación y paginación.
        /// </summary>
        /// <param name="filter">
        /// Filtros de búsqueda, incluyendo estado (Status), ubicación (Location),
        /// número de página (Page) y tamaño de página (PageSize).
        /// </param>
        /// <returns>Lista paginada de posiciones que coinciden con los criterios de búsqueda.</returns>
        /// <response code="200">Lista obtenida correctamente</response>
        /// <response code="400">Parámetros de búsqueda inválidos</response>
        /// <response code="401">Falta API Key</response>
        /// <response code="403">API Key inválida</response>
        [HttpGet("Filter")]
        [SwaggerOperation(Summary = "Obtiene posiciones filtradas", OperationId = "GetFilteredPositions")]
        [ProducesResponseType(typeof(IEnumerable<PositionDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll([FromQuery] PositionFilter filter)
        {
            var result = await _queryService.GetFilteredAsync(filter);
            return Ok(result);
        }
    }
}
