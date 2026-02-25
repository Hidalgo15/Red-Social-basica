using BookFace.Core.Domain.IRepository;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Domain.InterfacesRepository
{
    public interface ISolicitudAmistadRepository : IRepository<SolicitudAmistad>
    {
        // Obtener solicitudes de amistad recibidas por un usuario y con un estado específico (ej. Pendientes)
        Task<IEnumerable<SolicitudAmistad>> GetReceivedRequestsAsync(int receptorId, EstadoSolicitudAmistad? estado = null);

        // Obtener solicitudes de amistad enviadas por un usuario y con un estado específico (ej. Pendientes)
        Task<IEnumerable<SolicitudAmistad>> GetSentRequestsAsync(int remitenteId, EstadoSolicitudAmistad? estado = null);

        // Verificar si existe una solicitud de amistad entre dos usuarios con un estado específico (ej. Pendiente)
        // Es importante para evitar solicitudes duplicadas.
        Task<SolicitudAmistad> GetRequestBetweenUsersAsync(int userId1, int userId2, EstadoSolicitudAmistad? estado = null);

        // Si tu entidad SolicitudAmistad tiene un constructor que maneja RemitenteId y ReceptorId,
        // es posible que quieras un método que encuentre solicitudes por estos IDs combinados,
        // especialmente si tu lógica de negocio dicta que solo debe haber una solicitud pendiente.
        // Task<SolicitudAmistad> GetByRemitenteReceptorAndStatusAsync(int remitenteId, int receptorId, EstadoSolicitudAmistad estado);
    }
}
