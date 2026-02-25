using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repositories
{
    public class PublicacionRepository : RepositoryService<Publicacion> , IPublicacionRepository
    {
        private readonly ApplicationContext _context;

        public PublicacionRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Publicacion>> GetByUsuarioIdAsync(int usuarioId)
        {

            /*return await _context.Set<Publicacion>()
                .Include(p => p.UsuarioId) // Asumiendo que Publicacion tiene una propiedad de navegación 'Usuario' para el autor
                .Include(p => p.Comentarios) // Incluye los comentarios de la publicación
                    .ThenInclude(c => c.UsuarioId) // Y el usuario de cada comentario
                .Include(p => p.Likes) // Incluye los likes de la publicación
                .Where(p => p.UsuarioId == usuarioId) // Filtra por el ID del usuario
                .OrderByDescending(p => p.FechaCreacion) // Ordena por fecha de creación (más reciente primero)
                .ToListAsync();*/

            return await _context.Set<Publicacion>()
           // ELIMINAR: p.UsuarioId es un int, no una propiedad de navegación.
           // .Include(p => p.UsuarioId) 
           .Include(p => p.Comentarios) // Incluye los comentarios de la publicación
                                        // ELIMINAR: c.UsuarioId es un int. El usuario del comentario se cargará por separado.
                                        // .ThenInclude(c => c.UsuarioId) 
           .Include(p => p.Reacciones) // CORREGIDO: Se incluyen las Reacciones (la colección), no 'Likes' (el contador int)
           .Where(p => p.UsuarioId == usuarioId) // Filtra por el ID del usuario
           .OrderByDescending(p => p.FechaCreacion) // Ordena por fecha de creación (más reciente primero)
           .ToListAsync();
        }

        public override async Task<IEnumerable<Publicacion>> GetAllAsync()
        {
            /*  return await _context.Set<Publicacion>()
                  .Include(p => p.Comentarios)
                  .Include(p => p.Likes)
                  .ToListAsync();*/
            return await _context.Set<Publicacion>()
              .Include(p => p.Comentarios)
              .Include(p => p.Reacciones) // CORREGIDO: Se incluyen las Reacciones
              .ToListAsync();
        }

        public override async Task<Publicacion?> GetByIdAsync(int id)
        {
            /* return await _context.Set<Publicacion>()
                 .Include(p => p.Comentarios)
                 .Include(p => p.Likes)
                 .FirstOrDefaultAsync(p => p.Id == id); */

            return await _context.Set<Publicacion>()
            .Include(p => p.Comentarios)
            .Include(p => p.Reacciones) // CORREGIDO: Se incluyen las Reacciones
            .FirstOrDefaultAsync(p => p.Id == id);

        }
    }
}