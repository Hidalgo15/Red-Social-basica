
using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Domain.Entities
{
    public class Reaccion
    {
        public int Id { get; set; } // ID de la reacción
        public int PublicacionId { get; set; }
        public int UsuarioId { get; set; } // ID del usuario que realizó la reacción
        public Reacciones TipoReaccion { get; set; } // Usa tu enum Reacciones

    }
}
