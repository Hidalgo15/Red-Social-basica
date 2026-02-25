using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.GenericService;
using BookFace.Core.Application.ViewModel.ViewModel.Reaccion;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Enums;
using BookFace.Core.Domain.InterfacesRepository;

namespace BookFace.Core.Application.Service.EntidadService
{
    public class ReaccionService : GenericService<Reaccion, ReaccionSaveViewModel, ReaccionViewModel>, IReaccionService
    {

        private readonly IReaccionRepository _reaccionRepository;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public ReaccionService(IReaccionRepository reaccionRepository, IIdentityService identityService, IMapper mapper)
            : base(reaccionRepository, mapper)
        {
            _reaccionRepository = reaccionRepository;
            _identityService = identityService;
            _mapper = mapper;
        }

        public override async Task<List<ReaccionViewModel>> GetAllViewModel()
        {
            var reaccionesVm = await base.GetAllViewModel();
            foreach (var reaccionVm in reaccionesVm)
            {
                await PopulateUserDetails(reaccionVm);
            }
            return reaccionesVm;
        }

        public override async Task<ReaccionViewModel> GetByIdViewModel(int id)
        {
            var reaccionVm = await base.GetByIdViewModel(id);
            if (reaccionVm != null)
            {
                await PopulateUserDetails(reaccionVm);
            }
            return reaccionVm;
        }



        public async Task<List<ReaccionViewModel>> GetAllReaccionesForPublicacionAsync(int publicacionId)
        {
            var reacciones = await _reaccionRepository.GetAllReaccionesForPublicacionAsync(publicacionId);
            var reaccionesVm = _mapper.Map<List<ReaccionViewModel>>(reacciones.ToList());
            foreach (var reaccionVm in reaccionesVm)
            {
                await PopulateUserDetails(reaccionVm);
            }
            return reaccionesVm;
        }


        public async Task<(int likes, int dislikes)> GetReactionCountsForPublicacionAsync(int publicacionId)
        {
            var reacciones = await _reaccionRepository.GetAllReaccionesForPublicacionAsync(publicacionId);
            int likes = reacciones.Count(r => r.TipoReaccion == Reacciones.MeGusta);
            int dislikes = reacciones.Count(r => r.TipoReaccion == Reacciones.NoMeGusta);
            return (likes, dislikes);
        }

        public async Task<ReaccionViewModel?> GetUserReactionForPublicacionAsync(int publicacionId, int usuarioId)
        {
            var reaccion = await _reaccionRepository.GetByPublicacionAndUsuarioIdAsync(publicacionId, usuarioId);
            if (reaccion == null)
            {
                return null;
            }
            var reaccionVm = _mapper.Map<ReaccionViewModel>(reaccion);
            await PopulateUserDetails(reaccionVm); // Enriquecer con detalles del usuario
            return reaccionVm;
        }

        public async Task ProcessReactionAsync(ReaccionSaveViewModel vm)
        {
            var existingReaction = await _reaccionRepository.GetByPublicacionAndUsuarioIdAsync(vm.PublicacionId, vm.UsuarioId);

            if (existingReaction == null)
            {
                // No hay reacción previa, agregar una nueva
                await base.Add(vm);  // Llama al AddAsync del GenericService
            }
            else if (existingReaction.TipoReaccion == vm.TipoReaccion)
            {
                // El usuario hizo clic en la misma reacción nuevamente, la elimina (como un "quitar like")
                await _reaccionRepository.DeleteAsync(existingReaction.Id); // Asumiendo que DeleteAsync en IRepository acepta la entidad o un ID
                                                                         // Si DeleteAsync en el GenericService es por ID, tendrías que llamar a base.DeleteAsync(existingReaction.Id);
            }
            else
            {
                // El usuario cambió su reacción (de "MeGusta" a "NoMeGusta" o viceversa)
                existingReaction.TipoReaccion = vm.TipoReaccion;
                await _reaccionRepository.UpdateAsync(existingReaction); // Asumiendo UpdateAsync en IRepository acepta la entidad
            }
        }

        // --- Método Auxiliar ---
        private async Task PopulateUserDetails(ReaccionViewModel reaccionVm)
        {
            var userDetails = await _identityService.GetUserByIdAsync(reaccionVm.UsuarioId);
            if (userDetails != null)
            {
                reaccionVm.NombreUsuario = $"{userDetails.Nombre} {userDetails.Apellido}";
                reaccionVm.FotoPerfilUsuarioUrl = userDetails.FotoPerfilUrl;
            }
        }
    }
}
