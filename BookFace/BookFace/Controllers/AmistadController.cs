using BookFace.Core.Application.DTO;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.Service.EntidadService;
using BookFace.Core.Application.ViewModel.ViewModel.Amistad;
using BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookFace.Controllers
{
    [Authorize] // Asegura que solo usuarios autenticados puedan acceder
    [Route("[controller]")] // O "[controller]" si es un controlador MVC tradicional con vistas
    [Route("[controller]/[action]")]

    public class AmistadController : Controller
    {
        private readonly IAmistadService _amistadService;
        private readonly IIdentityService _identityService; // Para obtener el ID del usuario loggeado si no lo haces directamente desde Claims
        private readonly ISolicitudAmistadService _solicitudAmistadService;

        public AmistadController(IAmistadService amistadService, IIdentityService identityService, ISolicitudAmistadService solicitudAmistadService)
        {
            _amistadService = amistadService;
            _identityService = identityService;
            _solicitudAmistadService = solicitudAmistadService;// Puedes usarlo para validaciones o para obtener el UserId si tu base no te lo da
        
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }
            return int.Parse(userId);
        }

        // GET: Amigos/Index - Muestra la lista de amigos del usuario loggeado
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var friends = await _amistadService.GetAllFriendsByUserId(currentUserId);
                return View(friends);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para ver tus amigos.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar la lista de amigos: {ex.Message}";
                return View(new List<AmistadViewModel>());
            }
        }

        // GET: Amigos/Solicitudes - Muestra las solicitudes de amistad pendientes (recibidas y enviadas)
        [HttpGet("Solicitudes")]
        public async Task<IActionResult> Solicitudes()
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var receivedRequests = await _solicitudAmistadService.GetPendingReceivedRequests(currentUserId);
                var sentRequests = await _solicitudAmistadService.GetPendingSentRequests(currentUserId);

                var solicitudesVM = new SolicitudesAmistadCombinedViewModel
                {
                    ReceivedRequests = receivedRequests,
                    SentRequests = sentRequests
                };

                return View(solicitudesVM);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para ver las solicitudes de amistad.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar las solicitudes de amistad: {ex.Message}";
                return View(new SolicitudesAmistadCombinedViewModel());
            }
        }

        // GET: Amigos/Buscar - Muestra un formulario para buscar usuarios y enviar solicitudes
        [HttpGet("Buscar")]
        public async Task<IActionResult> Buscar(string searchTerm)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                List<UserDetailsDto> users = new List<UserDetailsDto>();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // ¡Aquí es donde usamos el nuevo método de IdentityService!
                    users = await _identityService.SearchUsersAsync(searchTerm, currentUserId);
                }

                ViewBag.SearchTerm = searchTerm;
                return View(users);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para buscar usuarios.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al buscar usuarios: {ex.Message}";
                return View(new List<UserDetailsDto>());
            }
        }

        // POST: Amigos/SendRequest/{targetUserId} - Envía una solicitud de amistad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(int targetUserId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var vm = new SolicitudAmistadSaveViewModel
                {
                    RemitenteId = currentUserId,
                    ReceptorId = targetUserId
                };

                await _solicitudAmistadService.SendFriendRequest(vm);
                TempData["SuccessMessage"] = "Solicitud de amistad enviada exitosamente.";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al enviar la solicitud de amistad: {ex.Message}";
            }
            return RedirectToAction("Buscar", new { searchTerm = "" });
        }

        // POST: Amigos/AcceptRequest/{requestId} - Acepta una solicitud de amistad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var solicitud = await _solicitudAmistadService.GetByIdViewModel(requestId);

                if (solicitud == null || solicitud.ReceptorId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Solicitud no encontrada o no tienes permiso para aceptarla.";
                    return RedirectToAction("Solicitudes");
                }

                await _solicitudAmistadService.AcceptFriendRequest(requestId);
                TempData["SuccessMessage"] = "Solicitud de amistad aceptada exitosamente. ¡Ahora son amigos!";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al aceptar la solicitud: {ex.Message}";
            }
            return RedirectToAction("Solicitudes");
        }

        // POST: Amigos/RejectRequest/{requestId} - Rechaza una solicitud de amistad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var solicitud = await _solicitudAmistadService.GetByIdViewModel(requestId);

                if (solicitud == null || solicitud.ReceptorId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Solicitud no encontrada o no tienes permiso para rechazarla.";
                    return RedirectToAction("Solicitudes");
                }

                await _solicitudAmistadService.RejectFriendRequest(requestId);
                TempData["SuccessMessage"] = "Solicitud de amistad rechazada.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al rechazar la solicitud: {ex.Message}";
            }
            return RedirectToAction("Solicitudes");
        }

        // POST: Amigos/CancelRequest/{requestId} - Cancela una solicitud de amistad enviada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var solicitud = await _solicitudAmistadService.GetByIdViewModel(requestId);

                if (solicitud == null || solicitud.RemitenteId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Solicitud no encontrada o no tienes permiso para cancelarla.";
                    return RedirectToAction("Solicitudes");
                }

                await _solicitudAmistadService.CancelFriendRequest(requestId);
                TempData["SuccessMessage"] = "Solicitud de amistad cancelada.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cancelar la solicitud: {ex.Message}";
            }
            return RedirectToAction("Solicitudes");
        }

        // POST: Amigos/Unfriend/{amistadId} - Elimina una amistad existente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfriend(int amistadId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var amistad = await _amistadService.GetByIdViewModel(amistadId);
                // Valida que la amistad exista y pertenezca al usuario actual
                if (amistad == null || (amistad.Usuario1.Id != currentUserId && amistad.Usuario2.Id != currentUserId))
                {
                    TempData["ErrorMessage"] = "Amistad no encontrada o no tienes permiso para eliminarla.";
                    return RedirectToAction("Index");
                }

                await _amistadService.Delete(amistadId);
                TempData["SuccessMessage"] = "Amistad eliminada exitosamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la amistad: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

    }
}
