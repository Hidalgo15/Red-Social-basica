using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.ViewModel.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BookFace.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService; // Inyectamos la interfaz del servicio
        private readonly IUrlHelperFactory _urlHelperFactory;
        public AccountController(IIdentityService identityService, IUrlHelperFactory urlHelperFactory)
        {
            _identityService = identityService;
            _urlHelperFactory = urlHelperFactory;
            // _mapper = mapper;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigir a Home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección CSRF
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Usamos el LoginAsync del servicio que devuelve SignInResult
            var result = await _identityService.LoginAsync(vm);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "¡Bienvenido de nuevo!";
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "La cuenta está bloqueada debido a demasiados intentos fallidos.");
                TempData["ErrorMessage"] = "Tu cuenta ha sido bloqueada temporalmente debido a demasiados intentos de inicio de sesión fallidos. Por favor, inténtalo de nuevo más tarde.";
                return View(vm);
            }
            else if (result.IsNotAllowed)
            {
                // Esto es si `EstaActivo` es false en tu IdentityService
                ModelState.AddModelError(string.Empty, "Tu cuenta no ha sido activada. Por favor, revisa tu correo electrónico para activarla.");
                TempData["ErrorMessage"] = "Tu cuenta no ha sido activada. Por favor, revisa tu correo electrónico para el enlace de activación.";
                return View(vm);
            }
            else if (result.RequiresTwoFactor)
            {
                // Si implementas 2FA, redirige a una vista para el código 2FA
                TempData["WarningMessage"] = "Tu cuenta requiere autenticación de dos factores.";
                return RedirectToAction("VerifyAuthenticatorCode", new { rememberMe = vm.RememberMe }); // Asume una acción VerifyAuthenticatorCode
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas. Usuario o contraseña incorrectos.");
                TempData["ErrorMessage"] = "Credenciales inválidas. Verifica tu nombre de usuario y contraseña.";
                return View(vm);
            }
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Crea un IUrlHelper para generar URLs absolutas
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);

            string scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.ToUriComponent();
            string origin = $"{scheme}://{host}";

            var result = await _identityService.RegisterUserAsync(vm, origin); // <-- Esto ya está bien

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "¡Registro exitoso! Por favor, revisa tu correo electrónico para activar tu cuenta.";
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Hubo un error durante el registro. Por favor, inténtalo de nuevo.";
                return View(vm);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [Authorize] // Solo los usuarios logueados pueden cerrar sesión
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _identityService.LogoutAsync();
            TempData["SuccessMessage"] = "Has cerrado sesión correctamente.";
            return RedirectToAction("Login");
        }

        
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Aquí es importante no revelar si el email existe por razones de seguridad (enumeración de usuarios)
            // El servicio ya maneja esto devolviendo IdentityResult.Success aunque el usuario no exista o no esté confirmado.
            var result = await _identityService.ForgotPasswordAsync(vm.Email);

            // Siempre mostramos un mensaje de éxito genérico para evitar dar pistas
            TempData["SuccessMessage"] = "Si tu dirección de correo electrónico está registrada en nuestro sistema, te enviaremos un enlace para restablecer tu contraseña.";
            return RedirectToAction(nameof(ForgotPassword)); // Redirige de nuevo a la vista con el mensaje de éxito
        }

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Token o ID de usuario inválido.");
            }
            var vm = new ResetPasswordViewModel { UserId = userId, Token = token };
            return View(vm);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _identityService.ResetPasswordAsync(vm);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tu contraseña ha sido restablecida con éxito. Ya puedes iniciar sesión con tu nueva contraseña.";
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Hubo un error al restablecer tu contraseña. El enlace podría ser inválido o haber caducado. Inténtalo de nuevo.";
                return View(vm);
            }
        }

        [HttpGet] // La acción de ConfirmEmail siempre será un GET
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                ViewBag.Success = false; // Estado para la vista
                ViewBag.Message = "Enlace de confirmación de email inválido o incompleto.";
                return View(); // Renderiza Views/Account/ConfirmEmail.cshtml
            }

            var result = await _identityService.ConfirmEmailAsync(userId, token);

            ViewBag.Success = result.Succeeded; // Pasa el estado a la vista

            if (!result.Succeeded)
            {
                ViewBag.Message = "Hubo un problema al confirmar tu cuenta. El enlace podría ser inválido o haber caducado.";
                // Opcional: Si quieres mostrar errores más específicos en la vista
                // ViewBag.Errors = result.Errors.Select(e => e.Description).ToList();
            }
            else
            {
                ViewBag.Message = "¡Tu cuenta ha sido activada con éxito! Ya puedes iniciar sesión.";
            }

            // La vista 'ConfirmEmail.cshtml' (en Views/Account) se renderizará con la información de ViewBag
            return View();
        }
    }
}
