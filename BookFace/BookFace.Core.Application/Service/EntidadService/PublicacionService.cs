using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.GenericService;
using BookFace.Core.Application.ViewModel.ViewModel.Publicacion;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.Enums;
using BookFace.Core.Domain.InterfacesRepository;


namespace BookFace.Core.Application.Service.EntidadService
{
    public class PublicacionService : GenericService<Publicacion,PublicacionSaveViewModel,PublicacionViewModel>, IPublicacionService
    {
        private readonly IPublicacionRepository _PublicacionRepository;
        private readonly IIdentityService _identityService; // Para obtener detalles de usuario
        private readonly IMapper _mapper;
        private readonly IReaccionService _reaccionService;

        public PublicacionService(IPublicacionRepository publicacionRepository, IIdentityService identityService, IMapper mapper, IReaccionService reaccionService)
            : base(publicacionRepository, mapper)
        {
            _PublicacionRepository = publicacionRepository;
            _identityService = identityService;
            _mapper = mapper;
            _reaccionService = reaccionService;
        }

        public async Task<List<PublicacionViewModel>> GetByUsuarioIdAsync(int usuarioId, int? currentLoggedInUserId = null)
        {
            var publicaciones = await _PublicacionRepository.GetByUsuarioIdAsync(usuarioId);
            var publicacionesVm = _mapper.Map<List<PublicacionViewModel>>(publicaciones.ToList());

            foreach (var publicacionVm in publicacionesVm)
            {
                await PopulatePublicacionDetails(publicacionVm, currentLoggedInUserId); // Pasa el ID del usuario actual
            }

            return publicacionesVm;
        }



        public async Task<PublicacionEditViewModel> GetPublicacionForEditAsync(int id)
        {
            // 1. Obtener la entidad Publicacion del repositorio
            var publicacion = await _PublicacionRepository.GetByIdAsync(id);
            if (publicacion == null)
            {
                return null;
            }

            // 2. Mapear la entidad Publicacion a PublicacionEditViewModel
            // Necesitarás una configuración de mapeo de Publicacion -> PublicacionEditViewModel
            return _mapper.Map<PublicacionEditViewModel>(publicacion);
        }

        public async Task <bool> UpdatePublicacionAsync(PublicacionEditViewModel vm)
        {
            // 1. Obtener la entidad Publicacion existente de la base de datos
            var publicacionToUpdate = await _PublicacionRepository.GetByIdAsync(vm.Id);
            if (publicacionToUpdate == null)
            {
                return false; // No encontrada
            }

            // 2. Mapear las propiedades editables del PublicacionEditViewModel a la entidad existente
            _mapper.Map(vm, publicacionToUpdate);

            // 3. Persistir los cambios en el repositorio
            await _PublicacionRepository.UpdateAsync(publicacionToUpdate);
            return true; // Actualización exitosa
        }


        public override async Task<List<PublicacionViewModel>> GetAllViewModel()
        {
            var publicacionesVm = await base.GetAllViewModel();

            foreach (var publicacionVm in publicacionesVm)
            {
                await PopulatePublicacionDetails(publicacionVm ,null);
            }

            return publicacionesVm;
        }

        public override async Task<PublicacionViewModel> GetByIdViewModel(int id)
        {
            var publicacionVm = await base.GetByIdViewModel(id);
            var currentUserId = _identityService.GetUserId();

            if (publicacionVm != null)
            {
                await PopulatePublicacionDetails(publicacionVm, currentUserId != null ? int.Parse(currentUserId) : (int?)null);

            }

            return publicacionVm;
        }


        private async Task PopulatePublicacionDetails(PublicacionViewModel publicacionVm, int? currentLoggedInUserId)
        {
            // 1. Enriquecer los detalles del autor de la publicación
            // 'publicacionVm.Usuario.Id' ya debería estar mapeado desde Publicacion.UsuarioId por AutoMapper (en PublicacionProfile).
            // Asumimos que 'publicacionVm.Usuario' no es null en este punto gracias a ese mapeo inicial.

            // 'autorDetails' será de tipo UserDetailsDto (de la capa Application),
            // devuelto por IdentityService después de mapear internamente desde ApplicationUser.
            var autorDetails = await _identityService.GetUserByIdAsync(publicacionVm.Usuario.Id);
            if (autorDetails != null)
            {
                // ***** CAMBIO PRINCIPAL AQUÍ *****
                // Usamos _mapper.Map para mapear las propiedades de 'autorDetails' (UserDetailsDto)
                // a la instancia existente de 'publicacionVm.Usuario' (UsuarioViewModel).
                // El mapeo CreateMap<UserDetailsDto, UsuarioViewModel>() que definiste en UsuarioProfile
                // se encarga de transferir Nombre, Apellido, NombreUsuario, FotoPerfilUrl, etc.
                _mapper.Map(autorDetails, publicacionVm.Usuario);

                // Las siguientes líneas de asignación manual YA NO SON NECESARIAS:
                // publicacionVm.Usuario.Nombre = $"{autorDetails.Nombre} {autorDetails.Apellido}";
                // publicacionVm.Usuario.FotoPerfilUrl = autorDetails.FotoPerfilUrl;
                // publicacionVm.Usuario.NombreUsuario = autorDetails.NombreUsuario;
            }

            // 2. Enriquecer los comentarios anidados (si PublicacionViewModel tiene una lista de ComentarioViewModel)
            if (publicacionVm.Comentarios != null && publicacionVm.Comentarios.Any())
            {
                foreach (var comentarioVm in publicacionVm.Comentarios)
                {
                    // Asegúrate de que comentarioVm.Usuario.Id ya esté mapeado correctamente
                    // (necesitarías un mapeo Comentario -> ComentarioViewModel que establezca Usuario.Id)
                    var autorComentarioDetails = await _identityService.GetUserByIdAsync(comentarioVm.Usuario.Id);
                    if (autorComentarioDetails != null)
                    {
                        // ***** CAMBIO PRINCIPAL AQUÍ (para comentarios) *****
                        // Similar al autor de la publicación, mapeamos el DTO del autor del comentario
                        // a la instancia existente de comentarioVm.Usuario.
                        _mapper.Map(autorComentarioDetails, comentarioVm.Usuario);

                        // Las siguientes líneas de asignación manual YA NO SON NECESARIAS:
                        // comentarioVm.Usuario.Nombre = $"{autorComentarioDetails.Nombre} {autorComentarioDetails.Apellido}";
                        // comentarioVm.Usuario.FotoPerfilUrl = autorComentarioDetails.FotoPerfilUrl;
                        // comentarioVm.Usuario.NombreUsuario = autorComentarioDetails.NombreUsuario; // Asegúrate de que esto mapea correctamente al usuario del comentario.
                    }

                    // Si los comentarios tienen respuestas anidadas, necesitarías una lógica recursiva aquí
                    // similar a la de ComentarioService.PopulateRepliesDetails
                }
            }

            // --- El resto del método permanece igual ---
            // 3. Enriquecer las Reacciones y calcular los contadores CantidadMeGusta y CantidadNoMeGusta
            // publicacionVm.Reacciones ya viene mapeado desde la entidad Publicacion si usas .Include()

            // Inicializa los contadores antes de recorrer las reacciones para asegurar que siempre empiecen en 0
            var (likes, dislikes) = await _reaccionService.GetReactionCountsForPublicacionAsync(publicacionVm.Id);
            publicacionVm.CantidadMeGusta = likes;
            publicacionVm.CantidadNoMeGusta = dislikes;

            // 4. Determinar el tipo de reacción del usuario actual usando ReaccionService
            publicacionVm.UserReactionType = null; // Inicializar a null (sin reacción)
            if (currentLoggedInUserId.HasValue)
            {
                var userReaction = await _reaccionService.GetUserReactionForPublicacionAsync(publicacionVm.Id, currentLoggedInUserId.Value);
                if (userReaction != null)
                {
                    publicacionVm.UserReactionType = userReaction.TipoReaccion;
                }
            }
        }
    }
}
 