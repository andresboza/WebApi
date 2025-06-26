namespace WebApi.DTOs
{
    /// <summary>
    /// Modelo de datos para una posición laboral.
    /// </summary>
    public class PositionDTO
    {
        /// <summary>
        /// Id del puesto.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Título del puesto. Máximo 100 caracteres.
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// Descripción detallada del puesto. Máximo 1000 caracteres.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Ubicación del puesto.
        /// </summary>
        public string Location { get; set; } = default!;

        /// <summary>
        /// Estado de la posición: draft, open, closed o archived.
        /// </summary>
        public string Status { get; set; } = default!;

        /// <summary>
        /// Identificador del reclutador asignado.
        /// </summary>
        public int RecruiterId { get; set; }

        /// <summary>
        /// Identificador del departamento correspondiente.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Presupuesto asignado a la posición.
        /// </summary>
        public decimal Budget { get; set; }

        /// <summary>
        /// Fecha límite para aplicar al puesto. Opcional.
        /// </summary>
        public DateTime? ClosingDate { get; set; }
    }
}
