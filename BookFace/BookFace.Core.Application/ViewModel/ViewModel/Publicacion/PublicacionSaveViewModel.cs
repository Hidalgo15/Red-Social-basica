using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Publicacion
{
    public class PublicacionSaveViewModel
    {
        [Required(ErrorMessage = "El contenido de la publicación es requerido.")]
        [StringLength(1000, ErrorMessage = "El contenido no puede exceder los 1000 caracteres.")]
        [Display(Name = "Contenido")]
        public string Contenido { get; set; }

        [Url(ErrorMessage = "El formato de la URL de la imagen no es válido.")]
        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        [Url(ErrorMessage = "El formato de la URL del video no es válido (ej. YouTube).")]
        [Display(Name = "URL de Video")]
        public string? VideoUrl { get; set; }

        // El UsuarioId normalmente se obtendría del usuario autenticado en la sesión,
        // pero se incluye aquí para el mapeo directo al constructor de la entidad.
        //[Required(ErrorMessage = "El ID del usuario es requerido.")]
       // [Range(1, int.MaxValue, ErrorMessage = "El ID del usuario debe ser válido.")]
        public int UsuarioId { get; set; }
    }
}
