namespace BookFace.Core.Domain.Entities
{
    public class Amistad
    {
        public int Id { get; private set; }
        public int UsuarioId1 { get; private set; } // ID del primer usuario en la amistad
        public int UsuarioId2 { get; private set; } // ID del segundo usuario en la amistad
        public DateTime FechaAmistad { get; private set; }

        // Propiedades de navegación
      //  public Usuario Usuario1 { get; private set; }
      //  public Usuario Usuario2 { get; private set; }

        // Constructor
        public Amistad(int usuarioId1, int usuarioId2)
        {
            if (usuarioId1 <= 0 || usuarioId2 <= 0) throw new ArgumentException("Los IDs de usuario deben ser válidos.");
            if (usuarioId1 == usuarioId2) throw new ArgumentException("Un usuario no puede ser amigo de sí mismo.");

            // Aseguramos un orden consistente para evitar duplicados (ej. (1,2) es igual que (2,1))
            UsuarioId1 = Math.Min(usuarioId1, usuarioId2);
            UsuarioId2 = Math.Max(usuarioId1, usuarioId2);
            FechaAmistad = DateTime.UtcNow;
        }

        // Constructor vacío para EF Core
        protected Amistad() { }
    }
}
