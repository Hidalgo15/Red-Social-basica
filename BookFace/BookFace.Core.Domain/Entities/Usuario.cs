namespace BookFace.Core.Domain.Entities
{
    /*public class Usuario 
    {
        public int Id { get; private set; } // Id es privado para asegurar que solo se asigne en el constructor o por ORM

        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
      //  public string NombreUsuario { get; private set; }
      //  public string CorreoElectronico { get; private set; }
        // La contraseña hash y salt idealmente no deberían estar en la entidad pública
        // ASP.NET Core Identity maneja esto internamente para Usuario, pero si no lo usaras, iría aquí.
        // Public string ContraseñaHash { get; private set; }
        // Public string ContraseñaSalt { get; private set; }

        public string? FotoPerfilUrl { get; private set; } // Puede ser nula
        public DateTime FechaRegistro { get; private set; }
        public bool EstaActivo { get; private set; } // Representa el estado de la cuenta (activada/desactivada)

        // Propiedades de navegación para las relaciones
        // Estas son listas de otras entidades con las que Usuario tiene una relación
        public ICollection<Publicacion> Publicaciones { get; private set; }
        public ICollection<Comentario> Comentarios { get; private set; }

        // Relaciones de amistad:
        // Un usuario puede ser el solicitante o el receptor de una amistad
        public ICollection<Amistad> AmistadesComoUsuario1 { get; private set; }
        public ICollection<Amistad> AmistadesComoUsuario2 { get; private set; }

        // Relaciones de solicitudes de amistad:
        public ICollection<SolicitudAmistad> SolicitudesEnviadas { get; private set; }
        public ICollection<SolicitudAmistad> SolicitudesRecibidas { get; private set; }


        // Constructor para crear un nuevo usuario (sin Id, ya que será generado por la DB)
        public Usuario(string nombre, string apellido, string nombreUsuario, string correoElectronico)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("El apellido no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(nombreUsuario)) throw new ArgumentException("El nombre de usuario no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(correoElectronico)) throw new ArgumentException("El correo electrónico no puede estar vacío.");

            Nombre = nombre;
            Apellido = apellido;
          //  NombreUsuario = nombreUsuario;
          //  CorreoElectronico = correoElectronico;
            FechaRegistro = DateTime.UtcNow; // Usar UTC para consistencia
            EstaActivo = false; // Por defecto, la cuenta no está activa hasta la confirmación por correo

            // Inicializar colecciones para evitar NullReferenceException
            Publicaciones = new List<Publicacion>();
            Comentarios = new List<Comentario>();
            AmistadesComoUsuario1 = new List<Amistad>();
            AmistadesComoUsuario2 = new List<Amistad>();
            SolicitudesEnviadas = new List<SolicitudAmistad>();
            SolicitudesRecibidas = new List<SolicitudAmistad>();
        }

        // Constructor para Entity Framework Core (requerido por algunos escenarios de EF Core para deserialización)
        // Puedes hacerlo 'protected internal' si quieres restringir su uso.
        protected Usuario()
        {
            // Constructor vacío para EF Core
            Publicaciones = new List<Publicacion>();
            Comentarios = new List<Comentario>();
            AmistadesComoUsuario1 = new List<Amistad>();
            AmistadesComoUsuario2 = new List<Amistad>();
            SolicitudesEnviadas = new List<SolicitudAmistad>();
            SolicitudesRecibidas = new List<SolicitudAmistad>();
        }

        // Método de comportamiento de dominio
        public void ActivarCuenta()
        {
            if (EstaActivo)
            {
                // Podrías lanzar una excepción o simplemente no hacer nada
                throw new InvalidOperationException("La cuenta ya está activa.");
            }
            EstaActivo = true;
        }

        public void ActualizarPerfil(string nuevoNombre, string nuevoApellido, string nuevaFotoPerfilUrl)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre)) throw new ArgumentException("El nombre no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(nuevoApellido)) throw new ArgumentException("El apellido no puede estar vacío.");

            Nombre = nuevoNombre;
            Apellido = nuevoApellido;
            FotoPerfilUrl = nuevaFotoPerfilUrl;
        }
        // ... otros métodos de comportamiento si los necesitas
    }*/
}
