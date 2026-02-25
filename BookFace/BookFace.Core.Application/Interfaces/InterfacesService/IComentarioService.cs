using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.Comentario;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IComentarioService : IGenericService<ComentarioSaveViewModel, ComentarioViewModel>
    {
        Task<List<ComentarioViewModel>> GetAllCommentsByPublicacionIdAsync(int publicacionId);

        // Obtener todas las respuestas a un comentario padre específico
        Task<List<ComentarioViewModel>> GetAllRepliesByParentCommentIdAsync(int parentCommentId);

        // NUEVO MÉTODO: Obtener todos los comentarios hechos por un usuario específico
        Task<List<ComentarioViewModel>> GetAllCommentsByUserIdAsync(int userId);

        Task<ComentarioViewModel> UpdateAndReturnAsync(ComentarioSaveViewModel vm);
    }
}
