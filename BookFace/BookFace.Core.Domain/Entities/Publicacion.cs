namespace BookFace.Core.Domain.Entities
{
    public class Publicacion
    {
        public int Id { get; private set; }
        public string Contenido { get; private set; }
        public string? ImagenUrl { get; private set; } // Puede ser nula
        public string? VideoUrl { get; private set; } // Puede ser nula, para enlaces de YouTube
        public int Likes { get; private set; } // Contador de likes, puedes cambiarlo a una colección si necesitas más detalles
        public DateTime FechaCreacion { get; private set; }
        public int UsuarioId { get; private set; } // Clave foránea al Usuario

        // Propiedad de navegación: Un Usuario tiene muchas Publicaciones
      //  public Usuario Usuario { get; private set; }

        // Propiedad de navegación: Una Publicacion tiene muchos Comentarios
        public ICollection<Comentario> Comentarios { get; private set; }
        public ICollection<Reaccion> Reacciones { get; private set; } = new List<Reaccion>();

        // Constructor para crear una nueva publicación
        public Publicacion(string contenido, int usuarioId, string? imagenUrl = null, string? videoUrl = null)
        {
            if (string.IsNullOrWhiteSpace(contenido)) throw new ArgumentException("El contenido de la publicación no puede estar vacío.");
            if (usuarioId <= 0) throw new ArgumentException("El ID de usuario debe ser válido.");

            Contenido = contenido;
            UsuarioId = usuarioId;
            ImagenUrl = imagenUrl;
            VideoUrl = videoUrl;
            FechaCreacion = DateTime.UtcNow;

            Comentarios = new List<Comentario>();
        }

        // Constructor vacío para Entity Framework Core
        protected Publicacion()
        {
            Comentarios = new List<Comentario>();
        }

        // Método de comportamiento de dominio: Editar publicación
        public void Editar(string nuevoContenido, string? nuevaImagenUrl, string? nuevoVideoUrl)
        {
            if (string.IsNullOrWhiteSpace(nuevoContenido)) throw new ArgumentException("El contenido de la publicación no puede estar vacío.");

            Contenido = nuevoContenido;
            ImagenUrl = nuevaImagenUrl;
            VideoUrl = nuevoVideoUrl;
            // No se actualiza FechaCreacion en la edición, se mantiene la original.
            // Si necesitaras un "FechaUltimaModificacion", lo añadirías como otra propiedad.
        }
    }
}
