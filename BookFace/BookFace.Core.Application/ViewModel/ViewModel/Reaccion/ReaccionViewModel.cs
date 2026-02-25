using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Application.ViewModel.ViewModel.Reaccion
{
    public class ReaccionViewModel
    {
        public int Id { get; set; }
        public int PublicacionId { get; set; }
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; } // Para mostrar quién reaccionó
        public string? FotoPerfilUsuarioUrl { get; set; } // Para mostrar foto de quién reaccionó
        public Reacciones TipoReaccion { get; set; }
    }
}
