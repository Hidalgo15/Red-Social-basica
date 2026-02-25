using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.GenericService;
using BookFace.Core.Application.ViewModel.ViewModel.Comentario;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;

namespace BookFace.Core.Application.Service.EntidadService
{
    public class ComentarioService : GenericService<Comentario, ComentarioSaveViewModel, ComentarioViewModel>, IComentarioService
    {

        private readonly IComentarioRepository _ComentarioRepository;
        private readonly IIdentityService _identityService; // Para obtener detalles de usuario
        private readonly IMapper _mapper;

        public ComentarioService(IComentarioRepository ComentarioRepository, IIdentityService identityService, IMapper mapper)
            : base(ComentarioRepository, mapper)
        {
            _ComentarioRepository = ComentarioRepository;
            _identityService = identityService;
            _mapper = mapper;
        }

        public override async Task<List<ComentarioViewModel>> GetAllViewModel()
        {
            var comentariosVm = await base.GetAllViewModel();

            foreach (var comentarioVm in comentariosVm)
            {
                await PopulateUserDetails(comentarioVm);
                await PopulateRepliesDetails(comentarioVm); // Asegúrate de enriquecer las respuestas también
            }

            return comentariosVm;
        }

        public override async Task<ComentarioViewModel> GetByIdViewModel(int id)
        {
            var comentarioVm = await base.GetByIdViewModel(id);

            if (comentarioVm != null)
            {
                await PopulateUserDetails(comentarioVm);
                await PopulateRepliesDetails(comentarioVm); // Enriquecer las respuestas
            }

            return comentarioVm;
        }


        public async Task<List<ComentarioViewModel>> GetAllCommentsByPublicacionIdAsync(int publicacionId)
        {
            // Usar el método GetByPublicacionIdAsync del repositorio
            var comentarios = await _ComentarioRepository.GetByPublicacionIdAsync(publicacionId);
            var comentariosVm = _mapper.Map<List<ComentarioViewModel>>(comentarios.ToList());

            foreach (var comentarioVm in comentariosVm)
            {
                await PopulateUserDetails(comentarioVm);
                await PopulateRepliesDetails(comentarioVm); // Enriquecer las respuestas
            }

            return comentariosVm;
        }

        public async Task<List<ComentarioViewModel>> GetAllCommentsByUserIdAsync(int userId)
        {
            // Necesitarás un método en IComentarioRepository (y su implementación)
            // para obtener comentarios por UsuarioId.
            // Asumo que tu entidad Comentario tiene una propiedad UsuarioId.
            // Si no existe, deberás añadir un método como GetCommentsByUserIdAsync en IComentarioRepository.
            var comentarios = await _ComentarioRepository.GetCommentsByUserIdAsync(userId); // <-- ASUMIDO que este método existe

            var comentariosVm = _mapper.Map<List<ComentarioViewModel>>(comentarios.ToList());

            foreach (var comentarioVm in comentariosVm)
            {
                await PopulateUserDetails(comentarioVm);
                await PopulateRepliesDetails(comentarioVm); // Enriquecer las respuestas
            }

            return comentariosVm;
        }

        public async Task<List<ComentarioViewModel>> GetAllRepliesByParentCommentIdAsync(int parentCommentId)
        {
            // Necesitarás un método en IComentarioRepository (y su implementación)
            // para obtener comentarios por ComentarioPadreId.
            // Asumo que tu entidad Comentario tiene una propiedad ComentarioPadreId.
            // Si no existe, deberás añadir un método como GetRepliesByParentCommentIdAsync en IComentarioRepository.
            var respuestas = await _ComentarioRepository.GetRepliesByParentCommentIdAsync(parentCommentId); // <-- ASUMIDO que este método existe

            var respuestasVm = _mapper.Map<List<ComentarioViewModel>>(respuestas.ToList());

            foreach (var respuestaVm in respuestasVm)
            {
                await PopulateUserDetails(respuestaVm);
                await PopulateRepliesDetails(respuestaVm); // Recursivamente enriquecer sub-respuestas
            }

            return respuestasVm;
        }


        public async Task<ComentarioViewModel> UpdateAndReturnAsync(ComentarioSaveViewModel vm)
        {
            // Actualiza el comentario usando la lógica existente
            await Update(vm); // Llama al método genérico para actualizar

            // Recupera el comentario actualizado y lo retorna
            var updated = await GetByIdViewModel(vm.Id);
            return updated;
        }


        // --- Métodos Auxiliares ---

        // Método auxiliar para poblar los detalles del usuario (autor del comentario)
        private async Task PopulateUserDetails(ComentarioViewModel comentarioVm)
        {
            var autorDetails = await _identityService.GetUserByIdAsync(comentarioVm.Usuario.Id);
            if (autorDetails != null)
            {
                comentarioVm.Usuario.Nombre = $"{autorDetails.Nombre} {autorDetails.Apellido}";
                comentarioVm.Usuario.FotoPerfilUrl = autorDetails.FotoPerfilUrl;
            }
        }


        // Método auxiliar para poblar los detalles de las respuestas recursivamente
        private async Task PopulateRepliesDetails(ComentarioViewModel comentarioVm)
        {
            if (comentarioVm.Respuestas != null && comentarioVm.Respuestas.Any())
            {
                foreach (var respuestaVm in comentarioVm.Respuestas)
                {
                    await PopulateUserDetails(respuestaVm);
                    // Llamada recursiva para las respuestas de las respuestas
                    await PopulateRepliesDetails(respuestaVm);
                }
            }
        }
    }
}