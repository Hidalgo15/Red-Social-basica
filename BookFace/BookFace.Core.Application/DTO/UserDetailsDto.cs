namespace BookFace.Core.Application.DTO
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string Email { get; set; } // ¡Añade esta línea!
    }
}
