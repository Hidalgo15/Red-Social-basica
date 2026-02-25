using BookFace.Core.Domain.IRepository;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Domain.InterfacesRepository
{
    public interface IPublicacionRepository : IRepository<Publicacion>
    {
        Task<IEnumerable<Publicacion>> GetByUsuarioIdAsync(int usuarioId);   
    }
}
