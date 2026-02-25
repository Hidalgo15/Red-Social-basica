using BookFace.Core.Domain.IRepository;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Domain.InterfacesRepository
{
    public interface IReaccionRepository : IRepository<Reaccion>
    {
        // Métodos específicos para reacciones
        Task<Reaccion?> GetByPublicacionAndUsuarioIdAsync(int publicacionId, int usuarioId);
        Task<IEnumerable<Reaccion>> GetAllReaccionesForPublicacionAsync(int publicacionId); // Opcional, para obtener todas las reacciones de una publicación
    }
}
