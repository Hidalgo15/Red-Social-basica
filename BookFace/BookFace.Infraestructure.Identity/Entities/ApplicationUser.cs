using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using BookFace.Core.Application.Interfaces;

namespace BookFace.Infraestructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser<int>  , IApplicationUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool EstaActivo { get; set; }

        public ICollection<Publicacion> Publicaciones { get; set; }
        public ICollection<Comentario> Comentarios { get; set; }
        public ICollection<Amistad> AmistadesComoUsuario1 { get; set; }
        public ICollection<Amistad> AmistadesComoUsuario2 { get; set; }
        public ICollection<SolicitudAmistad> SolicitudesEnviadas { get; set; }
        public ICollection<SolicitudAmistad> SolicitudesRecibidas { get; set; }
        public ICollection<Reaccion> Reacciones { get; set; } // <-- ¡NUEVA PROPIEDAD DE NAVEGACIÓN!




        public ApplicationUser() 
        {
            Publicaciones = new List<Publicacion>();
            Comentarios = new List<Comentario>();
            AmistadesComoUsuario1 = new List<Amistad>();
            AmistadesComoUsuario2 = new List<Amistad>();
            SolicitudesEnviadas = new List<SolicitudAmistad>();
            SolicitudesRecibidas = new List<SolicitudAmistad>();
            Reacciones = new List<Reaccion>(); // Inicializa la colección de reacciones
        }

        public ApplicationUser(string nombre, string apellido, string userName, 
                               string email, string? fotoPerfilUrl = null,
                               string? telefono = null) : base(userName)
        {
            Nombre = nombre;
            Apellido = apellido;
            Email = email; // Asigna el email también, ya que IdentityUser lo necesita
            FotoPerfilUrl = fotoPerfilUrl;
            Telefono = telefono;
            FechaRegistro = DateTime.UtcNow; // O asigna en el constructor vacío también
            EstaActivo = false; // Por defecto hasta confirmar email

            Publicaciones = new List<Publicacion>();
            Comentarios = new List<Comentario>();
            AmistadesComoUsuario1 = new List<Amistad>();
            AmistadesComoUsuario2 = new List<Amistad>();
            SolicitudesEnviadas = new List<SolicitudAmistad>();
            SolicitudesRecibidas = new List<SolicitudAmistad>();
            Reacciones = new List<Reaccion>(); // Inicializa la colección de reacciones

        }

        public void ActivarCuenta()
        {
            // Puedes usar EmailConfirmed = true; de IdentityUser, o tu propia EstaActivo
            if (EstaActivo)
            {
                throw new InvalidOperationException("La cuenta ya está activa.");
            }
            EstaActivo = true;
            // O EmailConfirmed = true;
        }

        public void ActualizarPerfil(string nuevoNombre, string nuevoApellido, string nuevaFotoPerfilUrl, string? nuevoTelefono)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre)) throw new ArgumentException("El nombre no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(nuevoApellido)) throw new ArgumentException("El apellido no puede estar vacío.");

            Nombre = nuevoNombre;
            Apellido = nuevoApellido;
            FotoPerfilUrl = nuevaFotoPerfilUrl;
            Telefono = nuevoTelefono;
        }
    }
}
