using BookFace.Core.Application.ViewModel.ViewModel.Publicacion; // Asegúrate de que esta sea la ruta correcta para tus ViewModels de Publicacion
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AutoMapper; // <-- Asegúrate de tener este using


namespace BookFace.Presentation.Web.Controllers
{
    [Authorize] // Todas las acciones de este controlador requerirán autenticación
    public class PublicacionController : Controller
    {
        private readonly IPublicacionService _publicacionService;
        private readonly IIdentityService _identityService; // Para obtener el ID del usuario actual
        private readonly IMapper _mapper; // Inyecta AutoMapper si lo necesitas para mapear entre ViewModels y entidades

        public PublicacionController(IPublicacionService publicacionService, IIdentityService identityService,IMapper mapper)
        {
            _publicacionService = publicacionService;
            _identityService = identityService;
            _mapper = mapper; // Asigna el mapper inyectado
        }

        // GET: /Post/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PublicacionSaveViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUserId = _identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] = "No se pudo identificar al usuario para crear la publicación.";
                return RedirectToAction("Login", "Account");
            }
            vm.UsuarioId = int.Parse(currentUserId);

            try
            {
                await _publicacionService.Add(vm); // Método heredado del GenericService
                TempData["SuccessMessage"] = "¡Publicación creada con éxito!";
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear la publicación: " + ex.Message;
                return View(vm);
            }
        }

      // GET: /Post/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // 1. Obtener la PublicacionViewModel completa para verificar la propiedad
            var publicacionVm = await _publicacionService.GetByIdViewModel(id);
            if (publicacionVm == null)
            {
                TempData["ErrorMessage"] = "Publicación no encontrada.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Comprobar si el usuario actual es el autor de la publicación
            var currentUserId = _identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId) || publicacionVm.Usuario.Id != int.Parse(currentUserId))
            {
                TempData["ErrorMessage"] = "No tienes permiso para editar esta publicación.";
                return RedirectToAction("Index", "Home");
            }

            // 3. Si está autorizado, mapear a PublicacionEditViewModel para mostrar el formulario de edición
            // Asegúrate de tener un mapeo configurado en AutoMapper de PublicacionViewModel a PublicacionEditViewModel
            var vm = _mapper.Map<PublicacionEditViewModel>(publicacionVm);
            return View(vm);
        }
        

        // POST: /Post/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PublicacionEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // 1. Revalidar que el usuario actual es el autor antes de intentar actualizar
            // Necesitamos el PublicacionViewModel completo para obtener el Usuario.Id para la autorización
            var existingPostForAuth = await _publicacionService.GetByIdViewModel(vm.Id);
            var currentUserId = _identityService.GetUserId();

            if (string.IsNullOrEmpty(currentUserId) || existingPostForAuth == null || existingPostForAuth.Usuario.Id != int.Parse(currentUserId))
            {
                TempData["ErrorMessage"] = "No tienes permiso para editar esta publicación o la publicación no existe.";
                return RedirectToAction("Index", "Home");
            }

            // 2. Intentar actualizar la publicación usando el servicio
            bool updateSuccess = await _publicacionService.UpdatePublicacionAsync(vm);

            if (updateSuccess)
            {
                TempData["SuccessMessage"] = "Publicación actualizada con éxito.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Este caso solo se daría si, por alguna razón,
                // la publicación no fue encontrada por el servicio al momento de actualizar,
                // a pesar de haberla encontrado para la comprobación de autorización.
                // Es un caso de error más general.
                TempData["ErrorMessage"] = "Error al actualizar la publicación. La publicación pudo no haber sido encontrada.";
                return View(vm);
            }
        }

        // POST: /Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // 1. Revalidar que el usuario actual es el autor antes de eliminar
                // Obtenemos el PublicacionViewModel completo para obtener el Usuario.Id para la autorización
                var existingPost = await _publicacionService.GetByIdViewModel(id); // Este SÍ puede lanzar KeyNotFoundException

                var currentUserId = _identityService.GetUserId();
                if (string.IsNullOrEmpty(currentUserId) || existingPost?.Usuario.Id != int.Parse(currentUserId))
                {
                    TempData["ErrorMessage"] = "No tienes permiso para eliminar esta publicación.";
                    return RedirectToAction("Index", "Home");
                }

                // 2. Si autorizado, proceder a eliminar
                await _publicacionService.Delete(id); // Este método no lanza KeyNotFoundException directamente
                TempData["SuccessMessage"] = "Publicación eliminada con éxito.";
            }
            catch (System.Exception ex) // Captura cualquier otra excepción general
            {
                // Aquí puedes loguear 'ex' para depuración
                TempData["ErrorMessage"] = "Error al eliminar la publicación: " + ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: /Post/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var publicacionVm = await _publicacionService.GetByIdViewModel(id);
            if (publicacionVm == null)
            {
                TempData["ErrorMessage"] = "Publicación no encontrada.";
                return RedirectToAction("Index", "Home");
            }
            return View(publicacionVm);
        }
    }
}