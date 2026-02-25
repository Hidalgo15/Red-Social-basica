using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Amistad
{
    public class AmistadViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Fecha de Amistad")]
        public DateTime FechaAmistad { get; set; }

        // Propiedades para mostrar la información de los dos usuarios en la amistad
        // Esto asume que tendrías un UsuarioViewModel para el mapeo anidado
        [Display(Name = "Usuario 1")]
        public UsuarioViewModel Usuario1 { get; set; }

        [Display(Name = "Usuario 2")]
        public UsuarioViewModel Usuario2 { get; set; }
    }
}
