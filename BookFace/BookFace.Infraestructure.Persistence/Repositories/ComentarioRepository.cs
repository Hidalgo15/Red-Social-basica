using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Infraestructure.Identity.Entities;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repositories
{
    public class ComentarioRepository : RepositoryService<Comentario>, IComentarioRepository
    {
        private readonly ApplicationContext _context;
        public ComentarioRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<Comentario>().AnyAsync(c => c.Id == id);
        }




        // Ejemplo: Modificación en ComentarioRepository (o en la base RepositoryService)
        public override async Task<Comentario?> GetByIdAsync(int id)
        {
            return await _context.Comentarios
                .Include(c => c.Respuestas) // Asegúrate de cargar las respuestas
                                            // Aquí puedes añadir .Include() para propiedades de navegación de Comentario a otros DOMINIO-entidades
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<Comentario>> GetAllAsync()
        {
            return await _context.Comentarios
                .Include(c => c.Respuestas) // Asegúrate de cargar las respuestas
                .ToListAsync();

        }

        public async Task<IEnumerable<Comentario>> GetByPublicacionIdAsync(int publicacionId)
        {
            return await _context.Comentarios
                  .Include(c => c.Respuestas) // IMPORTANTE: Incluir las respuestas para que se mapeen
                                               // .Include(c => _context.Set<ApplicationUser>().Where(u => u.Id == c.UsuarioId).FirstOrDefault()) // <-- Este Include es para la entidad ApplicationUser, pero el servicio poblará el ViewModel
                  .Where(c => c.PublicacionId == publicacionId)
                  .OrderBy(c => c.FechaCreacion)
                  .ToListAsync();
        }


        // En ComentarioRepository.cs
        public async Task<IEnumerable<Comentario>> GetRepliesByParentCommentIdAsync(int parentCommentId)
        {
            return await _context.Comentarios
                .Where(c => c.ComentarioPadreId == parentCommentId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comentario>> GetCommentsByUserIdAsync(int userId)
        {
            return await _context.Comentarios
                .Where(c => c.UsuarioId == userId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }
        // Aquí puedes implementar métodos específicos para ComentarioRepository si es necesario
    }
}

