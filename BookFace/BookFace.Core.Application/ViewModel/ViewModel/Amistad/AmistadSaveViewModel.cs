using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Amistad
{
    public class AmistadSaveViewModel
    {
        [Required(ErrorMessage = "El ID del primer usuario es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del primer usuario debe ser válido.")]
        [Display(Name = "ID Usuario 1")]
        public int UsuarioId1 { get; set; }

        [Required(ErrorMessage = "El ID del segundo usuario es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del segundo usuario debe ser válido.")]
        [Display(Name = "ID Usuario 2")]
        public int UsuarioId2 { get; set; }
    }
}
