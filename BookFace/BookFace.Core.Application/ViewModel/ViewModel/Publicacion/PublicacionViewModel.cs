using BookFace.Core.Application.ViewModel.ViewModel.Comentario;
using BookFace.Core.Application.ViewModel.ViewModel.Reaccion;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using BookFace.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace BookFace.Core.Application.ViewModel.ViewModel.Publicacion
{
    public class PublicacionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Contenido")]
        public string Contenido { get; set; }

        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Video (YouTube)")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Fecha de Publicación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

        // Información del usuario que hizo la publicación (necesita UsuarioViewModel)
        [Display(Name = "Publicado por")]
        public UsuarioViewModel Usuario { get; set; } // Asume que UsuarioViewModel ya existe

        // Colección de comentarios para esta publicación (necesita ComentarioViewModel)
        [Display(Name = "Comentarios")]
        public List<ComentarioViewModel> Comentarios { get; set; } // O ICollection<ComentarioViewModel>
        public int CantidadMeGusta { get; set; } // O simplemente 'Likes' si solo cuentas 'Me Gusta'
        public int CantidadNoMeGusta { get; set; } // Si tienes reacciones negativas
                                                  
        public List<ReaccionViewModel> Reacciones { get; set; } = new List<ReaccionViewModel>();

        // ¡PROPIEDAD ACTUALIZADA! Indica el tipo de reacción del usuario actual
        public Reacciones? UserReactionType { get; set; } // Será null si no ha reaccionado


    }
}

