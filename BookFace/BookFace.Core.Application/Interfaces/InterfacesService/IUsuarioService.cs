using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IUsuarioService : IGenericService<UsuarioSaveViewModel, UsuarioViewModel>
    {
        Task UpdateUser(UsuarioEditViewModel vm);
        Task<UsuarioEditViewModel> GetUserForEdit(int id);
    }
}
