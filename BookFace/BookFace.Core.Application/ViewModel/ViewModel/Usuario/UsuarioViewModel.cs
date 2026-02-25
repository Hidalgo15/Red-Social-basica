using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Usuario
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string CorreoElectronico { get; set; }

        [Display(Name = "URL de Foto de Perfil")]
        public string? FotoPerfilUrl { get; set; }

        [Display(Name = "Fecha de Registro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaRegistro { get; set; }

        [Display(Name = "Cuenta Activa")]
        public bool EstaActivo { get; set; }
    }
}
