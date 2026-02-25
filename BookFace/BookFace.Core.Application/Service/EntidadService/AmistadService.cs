using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.GenericService;
using BookFace.Core.Application.ViewModel.ViewModel.Amistad;
using BookFace.Core.Domain.Entities;
using BookFace.Core.Domain.InterfacesRepository;

namespace BookFace.Core.Application.Service.EntidadService
{
    public class AmistadService : GenericService <Amistad, AmistadSaveViewModel, AmistadViewModel> , IAmistadService
    {
        private readonly IAmistadRepository _amistadRepository;
        private readonly IIdentityService _identityService; // Para obtener detalles de usuario
        private readonly IMapper _mapper;

        public AmistadService(IAmistadRepository amistadRepository, IIdentityService identityService, IMapper mapper)
            :base (amistadRepository,mapper) 
        {
            _amistadRepository = amistadRepository;
            _identityService = identityService;
            _mapper = mapper;
        }

        public override async Task<List<AmistadViewModel>> GetAllViewModel()
        {
            // Llama a la implementación base para obtener la lista de ViewModels mapeados
            var amistadesVm = await base.GetAllViewModel();

            // Enriquecer cada ViewModel con los nombres y fotos de perfil de los usuarios
            foreach (var amistadVm in amistadesVm)
            {
                await PopulateUserDetails(amistadVm);
            }

            return amistadesVm;
        }

        // Sobrescribir GetByIdViewModel para enriquecer AmistadViewModel con datos de usuario
        public override async Task<AmistadViewModel> GetByIdViewModel(int id)
        {
            // Llama a la implementación base para obtener el ViewModel mapeado
            var amistadVm = await base.GetByIdViewModel(id);

            // Si se encontró, enriquecer con detalles del usuario
            if (amistadVm != null)
            {
                await PopulateUserDetails(amistadVm);
            }

            return amistadVm;
        }

        

        public async Task<bool> AreFriends(int userId1, int userId2)
        {
            // Usa directamente el método del repositorio
            return await _amistadRepository.AreFriendsAsync(userId1, userId2);
        }

        public async Task<List<AmistadViewModel>> GetAllFriendsByUserId(int userId)
        {
            // Usa GetByUsuarioIdAsync de tu repositorio de amistad
            var amistades = await _amistadRepository.GetByUsuarioIdAsync(userId);
            // Mapea la colección de entidades a ViewModels
            var amistadesVm = _mapper.Map<List<AmistadViewModel>>(amistades.ToList());

            // Enriquecer los ViewModels con los nombres y fotos de perfil de los usuarios
            foreach (var amistadVm in amistadesVm)
            {
                await PopulateUserDetails(amistadVm);
            }

            return amistadesVm;
        }



        private async Task PopulateUserDetails(AmistadViewModel amistadVm)
        {
            // Obtener los detalles de usuario para Usuario1
            var usuario1 = await _identityService.GetUserByIdAsync(amistadVm.Usuario1.Id);
            if (usuario1 != null)
            {
                amistadVm.Usuario1.Nombre = $"{usuario1.Nombre} {usuario1.Apellido}";
                amistadVm.Usuario1.FotoPerfilUrl = usuario1.FotoPerfilUrl;
            }
            // Obtener los detalles de usuario para Usuario2
            var usuario2 = await _identityService.GetUserByIdAsync(amistadVm.Usuario2.Id);
            if (usuario2 != null)
            {
                amistadVm.Usuario2.Nombre = $"{usuario2.Nombre} {usuario2.Apellido}";
                amistadVm.Usuario2.FotoPerfilUrl = usuario2.FotoPerfilUrl;
            }
        }

        public async Task<int> GetCommonFriendsCount(int userId1, int userId2)
        {
            // Paso 1: Obtener las amistades de userId1
            var friendsOfUser1Amistades = await _amistadRepository.GetByUsuarioIdAsync(userId1);
            // Extraer los IDs de los amigos de userId1
            var friendsOfUser1Ids = friendsOfUser1Amistades
                .Select(a => a.UsuarioId1 == userId1 ? a.UsuarioId2 : a.UsuarioId1)
                .ToHashSet(); // Usamos HashSet para una búsqueda más eficiente

            // Paso 2: Obtener las amistades de userId2
            var friendsOfUser2Amistades = await _amistadRepository.GetByUsuarioIdAsync(userId2);
            // Extraer los IDs de los amigos de userId2
            var friendsOfUser2Ids = friendsOfUser2Amistades
                .Select(a => a.UsuarioId1 == userId2 ? a.UsuarioId2 : a.UsuarioId1)
                .ToHashSet();

            // Paso 3: Excluir a los propios usuarios de sus listas de amigos si están allí
            // (Es decir, si userId1 es amigo de userId2, no queremos que userId2 sea contado como "amigo común" de userId1,
            // sino los amigos que tienen en común con una TERCERA persona)
            friendsOfUser1Ids.Remove(userId2); // userId2 no es un "amigo común" de userId1
            friendsOfUser2Ids.Remove(userId1); // userId1 no es un "amigo común" de userId2

            // Paso 4: Encontrar la intersección de los IDs de amigos
            var commonFriends = friendsOfUser1Ids.Intersect(friendsOfUser2Ids);

            return commonFriends.Count();
        }

        public async Task AddFriendship(int userId1, int userId2)
        { // Validar que no se intente ser amigo de uno mismo
            if (userId1 == userId2)
            {
                throw new InvalidOperationException("No puedes agregarte a ti mismo como amigo.");
            }

            // Verificar si ya son amigos para evitar duplicados
            bool alreadyFriends = await AreFriends(userId1, userId2);
            if (alreadyFriends)
            {
                throw new InvalidOperationException("Estos usuarios ya son amigos.");
            }

            // Normalizar el orden de los IDs para asegurar que la entrada en la DB sea única
            // y consistente (ej. siempre Usuario1Id < Usuario2Id)
            int u1 = Math.Min(userId1, userId2);
            int u2 = Math.Max(userId1, userId2);

            var amistadSaveVm = new AmistadSaveViewModel
            {
                UsuarioId1 = u1,
                UsuarioId2 = u2
            };

            // Utilizar el método Add de la clase base (GenericService) para persistir la amistad
            await base.Add(amistadSaveVm);
        }
    }
}