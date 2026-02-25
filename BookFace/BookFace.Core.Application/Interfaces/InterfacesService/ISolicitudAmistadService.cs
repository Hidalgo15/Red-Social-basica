using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad;
using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface ISolicitudAmistadService : IGenericService<SolicitudAmistadSaveViewModel, SolicitudAmistadViewModel>
    {
        Task<SolicitudAmistadViewModel> SendFriendRequest(SolicitudAmistadSaveViewModel vm);

        // Para aceptar una solicitud de amistad (por el ID de la solicitud)
        // Este método es crítico ya que también deberá interactuar con IAmistadService
        Task AcceptFriendRequest(int requestId);

        // Para rechazar una solicitud de amistad
        Task RejectFriendRequest(int requestId);

        // Para cancelar una solicitud de amistad (la que el usuario envió)
        Task CancelFriendRequest(int requestId);

        // Obtener solicitudes pendientes que un usuario ha recibido
        Task<List<SolicitudAmistadViewModel>> GetPendingReceivedRequests(int userId);

        // Obtener solicitudes pendientes que un usuario ha enviado
        Task<List<SolicitudAmistadViewModel>> GetPendingSentRequests(int userId);

        // Opcional: Obtener todas las solicitudes (enviadas/recibidas) de un usuario, filtradas por estado si es necesario
        Task<List<SolicitudAmistadViewModel>> GetAllRequestsForUser(int userId, EstadoSolicitudAmistad? estado = null);

        // Opcional: Verificar si ya existe una solicitud pendiente entre dos usuarios
        Task<bool> HasPendingRequest(int userId1, int userId2);
    }
}
