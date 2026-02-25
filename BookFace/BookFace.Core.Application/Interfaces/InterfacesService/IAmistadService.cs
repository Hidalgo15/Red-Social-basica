using BookFace.Core.Application.Interfaces.Generics;
using BookFace.Core.Application.ViewModel.ViewModel.Amistad;
using System.Web.Mvc;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IAmistadService : IGenericService<AmistadSaveViewModel, AmistadViewModel>
    {
        Task<List<AmistadViewModel>> GetAllFriendsByUserId(int userId);

        // Verificar si dos usuarios son amigos
        // Esto podría devolver un ViewModel o simplemente un bool
        Task<bool> AreFriends(int userId1, int userId2);

        Task AddFriendship(int userId1, int userId2);
        Task<int> GetCommonFriendsCount(int userId1, int userId2);

    }
}
