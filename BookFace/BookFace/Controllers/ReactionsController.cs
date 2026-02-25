using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.ViewModel.ViewModel.Reaccion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookFace.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden reaccionar
    [Route("[controller]")] // Puedes definir una ruta base para el controlador
    public class ReactionsController : Controller
    {
        private readonly ILogger<ReactionsController> _logger; // Asegúrate de tener esto inyectado
        private readonly IReaccionService _reaccionService;
        private readonly IIdentityService _identityService; // Para obtener el ID del usuario actual

        public ReactionsController(IReaccionService reaccionService, IIdentityService identityService, ILogger<ReactionsController> logger)
        {
            _reaccionService = reaccionService;
            _identityService = identityService;
            _logger = logger;
        }

        // POST: /Reactions/Process
        [HttpPost("Process")]
        [ValidateAntiForgeryToken] // Protección CSRF
        public async Task<IActionResult> Process([FromBody] ReaccionSaveViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Asegúrate de que el UsuarioId en el ViewModel sea el del usuario autenticado
            var currentUserId = _identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Usuario no autenticado.");
            }
            vm.UsuarioId = int.Parse(currentUserId);

            try
            {
                await _reaccionService.ProcessReactionAsync(vm);

                // Después de procesar la reacción, obtenemos los nuevos contadores
                var (likes, dislikes) = await _reaccionService.GetReactionCountsForPublicacionAsync(vm.PublicacionId);

                // Y el nuevo estado de reacción del usuario actual para esa publicación
                var userReaction = await _reaccionService.GetUserReactionForPublicacionAsync(vm.PublicacionId, vm.UsuarioId);

                return Ok(new
                {
                    likes = likes,
                    dislikes = dislikes,
                    userReactionType = userReaction?.TipoReaccion // Devuelve null si no hay reacción
                });
            }
            catch (System.Exception ex)
            {
                // Loguear el error (usar un ILogger si está configurado)
                _logger.LogError(ex, "Error al procesar la reacción para la publicación {PublicacionId} por el usuario {UsuarioId}", vm.PublicacionId, vm.UsuarioId);
                return StatusCode(500, "Error interno del servidor al procesar la reacción.");
            }
        }
    }
}
