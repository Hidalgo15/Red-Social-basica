using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.GenericService;
using BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Enums;
using BookFace.Core.Domain.InterfacesRepository;

namespace BookFace.Core.Application.Service.EntidadService
{
    public class SolicitudAmistadService : GenericService <SolicitudAmistad, SolicitudAmistadSaveViewModel,SolicitudAmistadViewModel>, ISolicitudAmistadService
    {

        private readonly ISolicitudAmistadRepository _solicitudAmistadRepository;
        private readonly IIdentityService _identityService; // Necesitas inyectar el servicio de identidad
        private readonly IAmistadService _amistadService; // Necesitas inyectar el servicio de amistad
        private readonly IMapper _mapper;
        

        public SolicitudAmistadService(
            IAmistadService amistadService,
            ISolicitudAmistadRepository solicitudAmistadRepository,
            IIdentityService identityService,
            IMapper mapper)
            :base(solicitudAmistadRepository, mapper) // Llama al constructor base con el repositorio y el mapper
        {
            _solicitudAmistadRepository = solicitudAmistadRepository;
            _identityService = identityService;
            _mapper = mapper;
            _amistadService = amistadService;
        }

        private async Task PopulateSolicitudDetails(SolicitudAmistadViewModel solicitudVm)
        {
            // Obtener detalles del remitente
            var remitenteDetails = await _identityService.GetUserByIdAsync(solicitudVm.RemitenteId);
            if (remitenteDetails != null)
            {
                solicitudVm.NombreRemitente = $"{remitenteDetails.Nombre} {remitenteDetails.Apellido}";
                solicitudVm.FotoPerfilRemitenteUrl = remitenteDetails.FotoPerfilUrl;
            }

            // Obtener detalles del receptor
            var receptorDetails = await _identityService.GetUserByIdAsync(solicitudVm.ReceptorId);
            if (receptorDetails != null)
            {
                solicitudVm.NombreReceptor = $"{receptorDetails.Nombre} {receptorDetails.Apellido}";
                solicitudVm.FotoPerfilReceptorUrl = receptorDetails.FotoPerfilUrl;
            }

            // Calcular amigos comunes si ambos IDs son válidos y diferentes
            if (solicitudVm.RemitenteId != 0 && solicitudVm.ReceptorId != 0)
            {
                solicitudVm.CantidadAmigosComunes = await _amistadService.GetCommonFriendsCount(solicitudVm.RemitenteId, solicitudVm.ReceptorId);
            }
        }

        // SendFriendRequest - Implementación completa
        public async Task<SolicitudAmistadViewModel> SendFriendRequest(SolicitudAmistadSaveViewModel vm)
        {
            // Validar que no se envíe una solicitud a sí mismo
            if (vm.RemitenteId == vm.ReceptorId)
            {
                throw new InvalidOperationException("No puedes enviarte una solicitud de amistad a ti mismo.");
            }

            // Verificar si ya existe una amistad
            bool areFriends = await _amistadService.AreFriends(vm.RemitenteId, vm.ReceptorId);
            if (areFriends)
            {
                throw new InvalidOperationException("Ya eres amigo de este usuario.");
            }

            // Verificar si ya existe una solicitud pendiente en cualquier dirección
            bool hasPending = await HasPendingRequest(vm.RemitenteId, vm.ReceptorId);
            if (hasPending)
            {
                throw new InvalidOperationException("Ya existe una solicitud de amistad pendiente con este usuario.");
            }

            // Crear la entidad SolicitudAmistad
            var solicitud = _mapper.Map<SolicitudAmistad>(vm);
            solicitud.Estado = EstadoSolicitudAmistad.Pendiente; // Establecer estado inicial
            solicitud.FechaSolicitud = DateTime.Now;

            // Guardar en el repositorio
            solicitud = await _solicitudAmistadRepository.AddAsync(solicitud);

            // Mapear la entidad guardada a ViewModel y poblar detalles adicionales
            var solicitudVm = _mapper.Map<SolicitudAmistadViewModel>(solicitud);
            await PopulateSolicitudDetails(solicitudVm);

            return solicitudVm;
        }

        // AcceptFriendRequest
        public async Task AcceptFriendRequest(int requestId)
        {
            var solicitud = await _solicitudAmistadRepository.GetByIdAsync(requestId);

            if (solicitud == null || solicitud.Estado != EstadoSolicitudAmistad.Pendiente)
            {
                throw new KeyNotFoundException("Solicitud de amistad no encontrada o no está en estado Pendiente.");
            }

            // Aquí, deberías añadir una validación para asegurar que el usuario actual es el ReceptorId de la solicitud
            // para la seguridad de tu aplicación.

            solicitud.Estado = EstadoSolicitudAmistad.Aceptada;
            await _solicitudAmistadRepository.UpdateAsync(solicitud);

            // Crear la amistad bilateral usando el servicio de amistad
            await _amistadService.AddFriendship(solicitud.RemitenteId, solicitud.ReceptorId);
        }

        // RejectFriendRequest
        public async Task RejectFriendRequest(int requestId)
        {
            var solicitud = await _solicitudAmistadRepository.GetByIdAsync(requestId);

            if (solicitud == null || solicitud.Estado != EstadoSolicitudAmistad.Pendiente)
            {
                throw new KeyNotFoundException("Solicitud de amistad no encontrada o no está en estado Pendiente.");
            }

            // Validar que el usuario actual es el ReceptorId si es necesario
            solicitud.Estado = EstadoSolicitudAmistad.Rechazada;
            await _solicitudAmistadRepository.UpdateAsync(solicitud);
        }

        // CancelFriendRequest
        public async Task CancelFriendRequest(int requestId)
        {
            var solicitud = await _solicitudAmistadRepository.GetByIdAsync(requestId);

            if (solicitud == null || solicitud.Estado != EstadoSolicitudAmistad.Pendiente)
            {
                throw new KeyNotFoundException("Solicitud de amistad no encontrada o no está en estado Pendiente.");
            }

            // Validar que el usuario actual es el RemitenteId si es necesario
            // La cancelación de una solicitud pendiente generalmente implica eliminarla.
            await _solicitudAmistadRepository.DeleteAsync(requestId);
        }

        // GetPendingReceivedRequests
        public async Task<List<SolicitudAmistadViewModel>> GetPendingReceivedRequests(int userId)
        {
            var solicitudes = await _solicitudAmistadRepository.GetAllAsync();
            var filteredSolicitudes = solicitudes
                                    .Where(s => s.ReceptorId == userId && s.Estado == EstadoSolicitudAmistad.Pendiente)
                                    .ToList();

            var solicitudesVm = _mapper.Map<List<SolicitudAmistadViewModel>>(filteredSolicitudes);

            foreach (var vm in solicitudesVm)
            {
                await PopulateSolicitudDetails(vm);
            }

            return solicitudesVm;
        }

        // GetPendingSentRequests
        public async Task<List<SolicitudAmistadViewModel>> GetPendingSentRequests(int userId)
        {
            var solicitudes = await _solicitudAmistadRepository.GetAllAsync();
            var filteredSolicitudes = solicitudes
                                    .Where(s => s.RemitenteId == userId && s.Estado == EstadoSolicitudAmistad.Pendiente)
                                    .ToList();

            var solicitudesVm = _mapper.Map<List<SolicitudAmistadViewModel>>(filteredSolicitudes);

            foreach (var vm in solicitudesVm)
            {
                await PopulateSolicitudDetails(vm);
            }

            return solicitudesVm;
        }

        // GetAllRequestsForUser
        public async Task<List<SolicitudAmistadViewModel>> GetAllRequestsForUser(int userId, EstadoSolicitudAmistad? estado = null)
        {
            var solicitudes = await _solicitudAmistadRepository.GetAllAsync();
            var query = solicitudes.AsQueryable()
                                .Where(s => s.RemitenteId == userId || s.ReceptorId == userId);

            if (estado.HasValue)
            {
                query = query.Where(s => s.Estado == estado.Value);
            }

            var solicitudesVm = _mapper.Map<List<SolicitudAmistadViewModel>>(query.ToList());

            foreach (var vm in solicitudesVm)
            {
                await PopulateSolicitudDetails(vm);
            }

            return solicitudesVm;
        }

        // HasPendingRequest
        public async Task<bool> HasPendingRequest(int userId1, int userId2)
        {
            var solicitudes = await _solicitudAmistadRepository.GetAllAsync();

            return solicitudes.Any(s =>
                s.Estado == EstadoSolicitudAmistad.Pendiente &&
                ((s.RemitenteId == userId1 && s.ReceptorId == userId2) ||
                 (s.RemitenteId == userId2 && s.ReceptorId == userId1)));
        }

        // *** Métodos de IGenericService (Sobreescritos para enriquecer los ViewModels) ***

        // Importante: Hemos quitado el `override Task<SolicitudAmistadSaveViewModel> Add` porque SendFriendRequest lo maneja.

        public override async Task<List<SolicitudAmistadViewModel>> GetAllViewModel()
        {
            var solicitudesVm = await base.GetAllViewModel();
            foreach (var vm in solicitudesVm)
            {
                await PopulateSolicitudDetails(vm);
            }
            return solicitudesVm;
        }

        public override async Task<SolicitudAmistadViewModel> GetByIdViewModel(int id)
        {
            var solicitudVm = await base.GetByIdViewModel(id);
            if (solicitudVm != null)
            {
                await PopulateSolicitudDetails(solicitudVm);
            }
            return solicitudVm;
        }
    }
}
