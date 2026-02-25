namespace BookFace.Core.Domain.Entities
{
    public class Comentario
    {
        public int Id { get; private set; }
        public string Contenido { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        public int UsuarioId { get; private set; } // Clave foránea al Usuario que comenta
        public int PublicacionId { get; private set; } // Clave foránea a la Publicación

        public int? ComentarioPadreId { get; private set; } // Clave foránea para comentarios anidados (puede ser nulo)

        // Propiedades de navegación
        // public Usuario Usuario { get; private set; }
        public Publicacion Publicacion { get; private set; }

        // Propiedades de navegación para comentarios anidados
        public Comentario? ComentarioPadre { get; private set; }
        public ICollection<Comentario> Respuestas { get; private set; } // Comentarios hijos

        // Constructor
        public Comentario(string contenido, int usuarioId, int publicacionId, int? comentarioPadreId = null)
        {
            if (string.IsNullOrWhiteSpace(contenido)) throw new ArgumentException("El contenido del comentario no puede estar vacío.");
            if (usuarioId <= 0) throw new ArgumentException("El ID de usuario debe ser válido.");
            if (publicacionId <= 0) throw new ArgumentException("El ID de publicación debe ser válido.");

            Contenido = contenido;
            UsuarioId = usuarioId;
            PublicacionId = publicacionId;
            ComentarioPadreId = comentarioPadreId;
            FechaCreacion = DateTime.UtcNow;

            Respuestas = new List<Comentario>();
        }

        // Constructor vacío para EF Core
        protected Comentario()
        {
            Respuestas = new List<Comentario>();
        }
    }
}

