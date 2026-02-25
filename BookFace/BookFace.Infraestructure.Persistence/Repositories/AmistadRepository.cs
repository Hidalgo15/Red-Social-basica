using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repositories
{
    public class AmistadRepository : RepositoryService<Amistad>, IAmistadRepository
    {
        private readonly ApplicationContext _context;
        public AmistadRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AreFriendsAsync(int usuarioId1, int usuarioId2)
        {
            // Para asegurar consistencia y evitar duplicados, guarda y busca siempre el ID menor como UsuarioId1
            // y el ID mayor como UsuarioId2. Tu lógica en SolicitudAmistadService para crear Amistad ya hace esto.
            int minId = Math.Min(usuarioId1, usuarioId2);
            int maxId = Math.Max(usuarioId1, usuarioId2);

            return await _context.Set<Amistad>()
                .AnyAsync(a => a.UsuarioId1 == minId && a.UsuarioId2 == maxId);
        }

        public async Task<IEnumerable<Amistad>> GetAllAmistadesAsync()
        {
            /* // Incluye las propiedades de navegación de los usuarios si necesitas sus datos
             // cuando recuperas las amistades (e.g., para mostrar sus nombres/fotos).
             return await _context.Set<Amistad>()
                 .Include(a => a.UsuarioId1) // Asumiendo que Amistad tiene estas propiedades de navegación
                 .Include(a => a.UsuarioId2)
                 .ToListAsync();
            */

            return await _context.Amistades.ToListAsync();
        }

        public async Task<IEnumerable<Amistad>> GetByUsuarioIdAsync(int usuarioId)
        {
            // Busca todas las amistades donde el 'usuarioId' dado es tanto UsuarioId1 como UsuarioId2
            return await _context.Set<Amistad>()
                /*.Include(a => a.UsuarioId1)
                .Include(a => a.UsuarioId2)
                .Where(a => a.UsuarioId1 == usuarioId || a.UsuarioId2 == usuarioId)
                .ToListAsync();*/
                .Where(a => a.UsuarioId1 == usuarioId || a.UsuarioId2 == usuarioId)
            .ToListAsync();


        }

        public async Task<Amistad> GetByUsuariosAsync(int usuarioId1, int usuarioId2)
        {
              // Usa la misma lógica de ordenamiento de IDs que en AreFriendsAsync para la búsqueda consistente
              int minId = Math.Min(usuarioId1, usuarioId2);
              int maxId = Math.Max(usuarioId1, usuarioId2);

            return await _context.Set<Amistad>()
            // .Include(a => a.Usuario1) // Línea eliminada
            // .Include(a => a.Usuario2) // Línea eliminada
            .FirstOrDefaultAsync(a => a.UsuarioId1 == minId && a.UsuarioId2 == maxId);



        }


    }
}
