using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.Reaccion;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IReaccionService : IGenericService<ReaccionSaveViewModel, ReaccionViewModel>
    {
        Task ProcessReactionAsync(ReaccionSaveViewModel vm);

        // Obtener la reacción de un usuario en una publicación específica
        Task<ReaccionViewModel?> GetUserReactionForPublicacionAsync(int publicacionId, int usuarioId);

        // Obtener el conteo de likes y dislikes para una publicación
        Task<(int likes, int dislikes)> GetReactionCountsForPublicacionAsync(int publicacionId);

        // Obtener todas las reacciones de una publicación (si PublicacionService lo necesita)
        Task<List<ReaccionViewModel>> GetAllReaccionesForPublicacionAsync(int publicacionId);

    }
}
