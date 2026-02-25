using BookFace.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Reaccion
{
    public class ReaccionSaveViewModel
    {
        public int Id { get; set; } // Necesario para Update, si actualizas una reacción existente
        [Required(ErrorMessage = "El ID de la publicación es requerido.")]
        public int PublicacionId { get; set; }

        [Required(ErrorMessage = "El ID del usuario es requerido.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de reacción es requerido.")]
        public Reacciones TipoReaccion { get; set; }
    }
}
