using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Comentario
{
    public class ComentarioSaveViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El contenido del comentario no puede estar vacío.")]
        [StringLength(500, ErrorMessage = "El contenido no puede exceder los 500 caracteres.")]
        [Display(Name = "Comentario")]
        public string Contenido { get; set; }

        [Required(ErrorMessage = "El ID de la publicación es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la publicación debe ser válido.")]
        public int PublicacionId { get; set; }

        // El UsuarioId normalmente se obtendría del usuario autenticado en la sesión,
        // pero se incluye aquí para el mapeo directo al constructor de la entidad.
        [Required(ErrorMessage = "El ID del usuario es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del usuario debe ser válido.")]
        public int UsuarioId { get; set; }

        // Opcional: El ID del comentario padre si este es una respuesta
        [Display(Name = "Responder a Comentario")]
        public int? ComentarioPadreId { get; set; }
    }
}
