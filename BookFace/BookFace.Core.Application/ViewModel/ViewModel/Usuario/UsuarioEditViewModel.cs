using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.Usuario
{
    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [RegularExpression(@"^[a-zA-Z0-9_.]*$", ErrorMessage = "El nombre de usuario solo puede contener letras, números, guiones bajos y puntos.")]
        public string NombreUsuario { get; set; } // Mapea a UserName de ApplicationUser

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        public string? FotoPerfilUrl { get; set; }
        public string? Telefono { get; set; }

        // Campos para cambio de contraseña (opcionales)
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string? ContrasenaActual { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? NuevaContrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "La nueva contraseña y su confirmación no coinciden.")]
        public string? ConfirmarContrasena { get; set; }
    }
}
