using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.ViewModel.ViewModel.Comentario;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace BookFace.Controllers
{
    [Authorize] // Asegura que solo usuarios autenticados puedan acceder a este controlador
    public class ComentarioController : Controller // Hereda de Controller para MVC
    {
        private readonly IComentarioService _comentarioService;
        private readonly IPublicacionService _publicacionService; // Necesario para redirigir a la publicación

        public ComentarioController(IComentarioService comentarioService, IPublicacionService publicacionService)
        {
            _comentarioService = comentarioService;
            _publicacionService = publicacionService;
        }

        // Método auxiliar para obtener el ID del usuario loggeado
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // Esto no debería ocurrir si [Authorize] funciona correctamente, pero es una buena práctica.
                throw new UnauthorizedAccessException("Usuario no autenticado o ID de usuario no disponible.");
            }
            return int.Parse(userIdClaim);
        }

        // POST: Comentario/Add
        // Esta acción se llamará desde un formulario en la vista de la publicación
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección CSRF
        public async Task<IActionResult> Add(ComentarioSaveViewModel vm)
        {
            vm.UsuarioId = GetCurrentUserId(); // <-- CORREGIDO: Mover aquí para asegurar que el ID esté siempre asignado

            // Validaciones iniciales (publicacionId o ComentarioPadreId)
            if (vm.PublicacionId > 0 && vm.ComentarioPadreId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Un comentario no puede ser a la vez de una publicación y una respuesta a otro comentario.");
            }
            if (vm.PublicacionId <= 0 && !vm.ComentarioPadreId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Un comentario debe estar asociado a una PublicacionId o a un ComentarioPadreId.");
            }

            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, redirigimos de vuelta a la publicación
                // y usamos TempData para mostrar los errores.
                TempData["ErrorMessage"] = "Error al agregar el comentario: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                // Redirigir a la publicación original
                return RedirectToAction("Detail", "Publicacion", new { id = vm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }

            try
            {
                await _comentarioService.Add(vm);

                TempData["SuccessMessage"] = "Comentario agregado exitosamente.";
                return RedirectToAction("Detail", "Publicacion", new { id = vm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", "Publicacion", new { id = vm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", "Publicacion", new { id = vm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno al agregar el comentario: {ex.Message}";
                return RedirectToAction("Detail", "Publicacion", new { id = vm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var comentarioVm = await _comentarioService.GetByIdViewModel(id);
                if (comentarioVm == null)
                {
                    TempData["ErrorMessage"] = "Comentario no encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                if (comentarioVm.Usuario.Id != GetCurrentUserId())
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este comentario.";
                    return RedirectToAction("Detalle", "Publicacion", new { id = comentarioVm.PublicacionId }); // CORREGIDO: "Publicacion" en singular
                }

                var saveVm = new ComentarioSaveViewModel
                {
                    Id = comentarioVm.Id,
                    Contenido = comentarioVm.Contenido,
                    PublicacionId = comentarioVm.PublicacionId,
                    UsuarioId = comentarioVm.Usuario.Id,
                    ComentarioPadreId = comentarioVm.ComentarioPadreId
                };

                return View(saveVm);
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el comentario para edición: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Comentario/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ComentarioSaveViewModel vm)
        {
            vm.UsuarioId = GetCurrentUserId(); // Asegura que el UsuarioId no se manipule desde el cliente

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error al editar el comentario: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(vm);
            }

            try
            {
                var existingComment = await _comentarioService.GetByIdViewModel(vm.Id);
                if (existingComment == null)
                {
                    TempData["ErrorMessage"] = "Comentario no encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                if (existingComment.Usuario.Id != GetCurrentUserId())
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este comentario.";
                    return RedirectToAction("Detalle", "Publicacion", new { id = existingComment.PublicacionId }); // CORREGIDO: "Publicacion" en singular
                }

                await _comentarioService.Update(vm);

                TempData["SuccessMessage"] = "Comentario editado exitosamente.";
                return RedirectToAction("Detalle", "Publicacion", new { id = existingComment.PublicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(vm);
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno al editar el comentario: {ex.Message}";
                return View(vm);
            }
        }

        // POST: Comentario/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int publicacionId)
        {
            try
            {
                var existingComment = await _comentarioService.GetByIdViewModel(id);
                if (existingComment == null)
                {
                    TempData["ErrorMessage"] = "Comentario no encontrado para eliminar.";
                    return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
                }

                if (existingComment.Usuario.Id != GetCurrentUserId())
                {
                    TempData["ErrorMessage"] = "No tienes permiso para eliminar este comentario.";
                    return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
                }

                await _comentarioService.Delete(id);

                TempData["SuccessMessage"] = "Comentario eliminado exitosamente.";
                return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error interno al eliminar el comentario: {ex.Message}";
                return RedirectToAction("Detalle", "Publicacion", new { id = publicacionId }); // CORREGIDO: "Publicacion" en singular
            }
        }
    }
}