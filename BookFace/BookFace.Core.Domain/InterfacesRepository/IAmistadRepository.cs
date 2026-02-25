using BookFace.Core.Domain.IRepository;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Domain.InterfacesRepository
{
    public interface IAmistadRepository : IRepository<Amistad>
    {
        Task<Amistad> GetByUsuariosAsync(int usuarioId1, int usuarioId2);
        Task<IEnumerable<Amistad>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Amistad>> GetAllAmistadesAsync();
        Task<bool> AreFriendsAsync(int usuarioId1, int usuarioId2);
    }
}