using AutoMapper;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.ViewModel.ViewModel.Home;
using BookFace.Core.Application.ViewModel.ViewModel.Publicacion;
using BookFace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using System.Diagnostics;

namespace BookFace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIdentityService _identityService;
        private readonly IPublicacionService _postService;
        private readonly IAmistadService _friendService;
        private readonly IMapper _mapper; // Inyecta AutoMapper

        public HomeController(
            ILogger<HomeController> logger,
            IIdentityService identityService,
            IPublicacionService postService,
            IAmistadService friendService,
            IMapper mapper)
        {
            _logger = logger;
            _identityService = identityService;
            _postService = postService;
            _friendService = friendService;
            _mapper = mapper;
        }

        // GET: /Home/Index
        // Esta será la pantalla principal del feed de la red social
        [Authorize] // Solo usuarios autenticados pueden ver el feed
        public async Task<IActionResult> Index()
        {
            var currentUserId = _identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                // Esto no debería ocurrir con [Authorize], pero como seguridad extra.
                TempData["ErrorMessage"] = "No se pudo identificar al usuario. Por favor, inicie sesión.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(currentUserId);

            // 1. Obtener los IDs de los amigos del usuario actual
            // Suponiendo que quieres los IDs de los amigos (Usuario2)
            var friendIds = (await _friendService.GetAllFriendsByUserId(userId))
                ?.Select(a => a.Usuario2.Id)
                .ToList() ?? new List<int>();

            var userAndFriendIds = new List<int> { userId };
            userAndFriendIds.AddRange(friendIds);

            // 2. Obtener las publicaciones de los amigos y del propio usuario
            // Asume que GetPostsByUserIdsAsync es un método en IPostService que acepta una lista de IDs
            // 2. Obtener las publicaciones de los amigos y del propio usuario
            var postDtos = new List<PublicacionViewModel>();
            foreach (var id in userAndFriendIds)
            {
                var postsDeUsuario = await _postService.GetByUsuarioIdAsync(id, userId);
                if (postsDeUsuario != null)
                    postDtos.AddRange(postsDeUsuario);
            }

            // 3. Mapear los DTOs a ViewModels
            var posts = _mapper.Map<List<PublicacionViewModel>>(postDtos);

            // Ordenar las publicaciones por fecha descendente
            posts = posts.OrderByDescending(p => p.FechaCreacion).ToList();

            // Preparar el HomeViewModel
            var homeViewModel = new HomeViewModel
            {
                Posts = posts,
                // Puedes añadir más propiedades si son necesarias para la vista,
                // como la información del usuario actual o un campo para crear una nueva publicación rápida.
                CurrentUserName = _identityService.GetUserName() // Asume que IIdentityService tiene este método
            };

            // Puedes agregar un TempData si se viene de un registro exitoso, por ejemplo.
            if (TempData["RegistrationSuccess"] != null)
            {
                ViewBag.ShowWelcomeAlert = true; // Para mostrar un mensaje de bienvenida solo una vez
            }

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
