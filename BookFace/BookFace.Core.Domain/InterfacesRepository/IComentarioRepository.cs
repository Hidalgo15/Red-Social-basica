using BookFace.Core.Domain.IRepository;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Domain.InterfacesRepository
{
    public interface IComentarioRepository : IRepository<Comentario>
    {
        Task<IEnumerable<Comentario>> GetByPublicacionIdAsync(int publicacionId);
        Task<bool> ExistsAsync(int id);

        // En IComentarioRepository
        Task<IEnumerable<Comentario>> GetRepliesByParentCommentIdAsync(int parentCommentId);
        Task<IEnumerable<Comentario>> GetCommentsByUserIdAsync(int userId);
    }
}
