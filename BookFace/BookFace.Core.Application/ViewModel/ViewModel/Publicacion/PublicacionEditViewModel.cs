using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Publicacion
{
    public class PublicacionEditViewModel
    {
        [Required(ErrorMessage = "El ID de la publicación es requerido.")]
        public int Id { get; set; } // Necesario para identificar la publicación a editar

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
    }
}
