using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Enums;
using BookFace.Core.Domain.InterfacesRepository;
using BookFace.Infraestructure.Persistence.DBContext;
using BookFace.Infraestructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookFace.Infraestructure.Persistence.Repositories
{
    public class SolicitudAmistadRepository : RepositoryService<SolicitudAmistad>, ISolicitudAmistadRepository
    {

        private readonly ApplicationContext _context;
        public SolicitudAmistadRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SolicitudAmistad>> GetReceivedRequestsAsync(int receptorId, EstadoSolicitudAmistad? estado = null)
        {
            IQueryable<SolicitudAmistad> query = _context.Set<SolicitudAmistad>() // Usamos _context.Set<TEntity>()
             .Include(s => s.RemitenteId) // Cargar la entidad del usuario remitente
             .Include(s => s.ReceptorId); // Cargar la entidad del usuario receptor

            query = query.Where(s => s.ReceptorId == receptorId);

            if (estado.HasValue)
            {
                query = query.Where(s => s.Estado == estado.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<SolicitudAmistad> GetRequestBetweenUsersAsync(int userId1, int userId2, EstadoSolicitudAmistad? estado = null)
        {
            IQueryable<SolicitudAmistad> query = _context.Set<SolicitudAmistad>(); // Usamos _context.Set<TEntity>()

            // Buscar solicitudes en ambas direcciones (userId1 -> userId2 o userId2 -> userId1)
            query = query.Where(s =>
                (s.RemitenteId == userId1 && s.ReceptorId == userId2) ||
                (s.RemitenteId == userId2 && s.ReceptorId == userId1)
            );

            if (estado.HasValue)
            {
                query = query.Where(s => s.Estado == estado.Value);
            }

            // Usar FirstOrDefaultAsync ya que esperamos como máximo una solicitud única
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SolicitudAmistad>> GetSentRequestsAsync(int remitenteId, EstadoSolicitudAmistad? estado = null)
        {
            IQueryable<SolicitudAmistad> query = _context.Set<SolicitudAmistad>() // Usamos _context.Set<TEntity>()
            .Include(s => s.RemitenteId) // Cargar la entidad del usuario remitente
            .Include(s => s.ReceptorId); // Cargar la entidad del usuario receptor

            query = query.Where(s => s.RemitenteId == remitenteId);

            if (estado.HasValue)
            {
                query = query.Where(s => s.Estado == estado.Value);
            }

            return await query.ToListAsync();
        }
    }
}
