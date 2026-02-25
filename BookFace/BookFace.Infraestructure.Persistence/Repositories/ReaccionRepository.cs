using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repositories
{
    public class ReaccionRepository : RepositoryService<Reaccion>, IReaccionRepository
    {

        private readonly ApplicationContext _context;
        public ReaccionRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Reaccion>> GetAllReaccionesForPublicacionAsync(int publicacionId)
        {
            return await _context.Set<Reaccion>()
                 .Where(r => r.PublicacionId == publicacionId)
                 .ToListAsync(); // Devuelve List<Reaccion> que es compatible con IEnumerable<Reaccion>
        }

        public async Task<Reaccion?> GetByPublicacionAndUsuarioIdAsync(int publicacionId, int usuarioId)
        {
            return await _context.Set<Reaccion>()
                 .FirstOrDefaultAsync(r => r.PublicacionId == publicacionId && r.UsuarioId == usuarioId);
        }
    }
}
