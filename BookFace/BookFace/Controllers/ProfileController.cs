using BookFace.Core.Application.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;


namespace BookFace.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper; // Necesario para mapear entre entidad/DTO y ViewModel
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IIdentityService identityService, IMapper mapper, ILogger<ProfileController> logger)
        {
            _identityService = identityService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: /Profile/Index
        // Muestra el perfil del usuario actual
        public async Task<IActionResult> Index(int? profileUserId = null) // Añadimos un parámetro opcional para ver otros perfiles
        {
            string currentUserIdString = _identityService.GetUserId();
            if (currentUserIdString == null)
            {
                TempData["ErrorMessage"] = "No se pudo encontrar la información del usuario. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Login", "Account");
            }

            int userIdToFetch;
            // Si se pasa un profileUserId, se intenta ver ese perfil; de lo contrario, se ve el del usuario actual.
            if (profileUserId.HasValue)
            {
                userIdToFetch = profileUserId.Value;
            }
            else
            {
                userIdToFetch = int.Parse(currentUserIdString);
            }

            // 1. Obtener los detalles del usuario como DTO
            // GetUserByIdAsync DEBE devolver UserDetailsDto
            var userDetailsDto = await _identityService.GetUserByIdAsync(userIdToFetch);

            if (userDetailsDto == null)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el perfil. El usuario no existe o no se encontró.";
                _logger.LogWarning("Perfil de usuario no encontrado para ID: {UserId}", userIdToFetch);
                return RedirectToAction("Index", "Home");
            }

            // 2. ¡Mapear de UserDetailsDto a UsuarioViewModel!
            var usuarioViewModel = _mapper.Map<BookFace.Core.Application.ViewModel.ViewModel.Usuario.UsuarioViewModel>(userDetailsDto);

            // 3. Pasar el ViewModel mapeado a la vista
            return View(usuarioViewModel);
        }

        // GET: /Profile/Edit
        // Muestra el formulario para editar el perfil del usuario actual
        public async Task<IActionResult> Edit()
        {
            var userId = _identityService.GetUserId();
            if (userId == null)
            {
                TempData["ErrorMessage"] = "No se pudo encontrar la información del usuario para editar. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Login", "Account");
            }

            var userProfile = await _identityService.GetUserByIdAsync(int.Parse(userId));
            if (userProfile == null)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el perfil para editar. El usuario no existe o no se encontró.";
                _logger.LogWarning("Perfil de usuario no encontrado para edición para ID: {UserId}", userId);
                return RedirectToAction("Index", "Home");
            }

            // Mapear de UsuarioViewModel a UsuarioEditViewModel para pre-llenar el formulario
            var editViewModel = _mapper.Map<UsuarioEditViewModel>(userProfile);

            // Si tienes un ID en UsuarioEditViewModel que es string (como Identity), asegúrate de que se asigne correctamente
            editViewModel.Id = int.Parse(userId); // Asegúrate de que el ID en el VM sea int si es necesario

            return View(editViewModel);
        }

        // POST: /Profile/Edit
        // Procesa el formulario de edición de perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditViewModel vm)
        {
            // Validar el ViewModel antes de intentar actualizar
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario.";
                return View(vm);
            }

            // Asignar el ID del usuario actual al ViewModel, ya que no vendrá directamente del formulario
            var currentUserId = _identityService.GetUserId();
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Error de autenticación. Por favor, inicie sesión de nuevo.";
                return RedirectToAction("Login", "Account");
            }
            vm.Id = int.Parse(currentUserId);

            // Llama al servicio para actualizar las propiedades del usuario
            var result = await _identityService.UpdateIdentityUserPropertiesAsync(vm);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tu perfil ha sido actualizado exitosamente.";
                return RedirectToAction(nameof(Index)); // Redirige a la vista del perfil actualizado
            }
            else
            {
                // Si hay errores de Identity (ej. nombre de usuario ya existe, email inválido)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Hubo un error al actualizar tu perfil. Por favor, verifica los datos e inténtalo de nuevo.";
                _logger.LogError("Error al actualizar perfil para usuario ID {UserId}: {Errors}", vm.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                return View(vm);
            }
        }
    }
}