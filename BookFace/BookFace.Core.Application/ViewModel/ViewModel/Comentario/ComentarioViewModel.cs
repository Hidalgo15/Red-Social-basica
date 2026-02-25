using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Comentario
{
    public class ComentarioViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Contenido")]
        public string Contenido { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

        // Información del usuario que hizo el comentario (necesita UsuarioViewModel)
        [Display(Name = "Comentado por")]
        public UsuarioViewModel Usuario { get; set; }

        // ID de la Publicación a la que pertenece este comentario
        public int PublicacionId { get; set; }

        // ID del comentario padre si este es una respuesta (nulo para comentarios de nivel superior)
        public int? ComentarioPadreId { get; set; }

        // Para comentarios anidados: colección de respuestas a este comentario
        [Display(Name = "Respuestas")]
        public ICollection<ComentarioViewModel> Respuestas { get; set; }
    }
}
