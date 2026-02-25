using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.Publicacion;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IPublicacionService : IGenericService<PublicacionSaveViewModel, PublicacionViewModel>
    {
        // Método adicional para el EditViewModel específico de Publicacion
        Task <bool>UpdatePublicacionAsync(PublicacionEditViewModel vm);
        Task<PublicacionEditViewModel> GetPublicacionForEditAsync(int id);

        // Método adicional para obtener publicaciones por usuario, si no está ya en IGenericService o si necesitas enriquecimiento específico
        Task<List<PublicacionViewModel>> GetByUsuarioIdAsync(int usuarioId, int? currentLoggedInUserId);

    }
}