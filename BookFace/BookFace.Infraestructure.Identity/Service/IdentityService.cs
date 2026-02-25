using AutoMapper;
using BookFace.Core.Application.DTO;
using BookFace.Core.Application.Identity.Interfaces;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Application.ViewModel.ViewModel.Auth;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using BookFace.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BookFace.Infraestructure.Identity.Service
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor; // Para GetUserId/GetUserName
        private readonly IEmailService _emailService; // <-- ¡Inyecta el servicio de email!
        private readonly ILogger<IdentityService> _logger; // <-- Inyecta el logger


        public IdentityService(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               IMapper mapper,
                               IHttpContextAccessor httpContextAccessor,
                               IEmailService emailService,
                               ILogger<IdentityService> logger )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService; // Asigna el servicio de email
            _logger = logger; // Asigna el logger
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {

            var response = new LoginResponseDto
            {
                Id = string.Empty,        // O un valor predeterminado adecuado, si un ID nulo tiene sentido para un fallo.
                Name = string.Empty,
                LastName = string.Empty,
                Email = string.Empty,
                UserName = string.Empty,
                HasError = false,         // Esto ya lo tenías
                Errors = new List<string>() // Esto ya lo tenías
            };
            
     

            // 1. Encontrar usuario por UserName
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("Credenciales inválidas. Usuario o contraseña incorrectos.");
                _logger.LogWarning("Intento de login fallido para el usuario: {UserName}. Usuario no encontrado.", loginDto.UserName);
                return response;
            }

            // 2. Verificar si la cuenta está activa (EstaActivo)
            // Asumo que tu ApplicationUser tiene una propiedad EstaActivo
            if (!user.EstaActivo)
            {
                response.HasError = true;
                response.Errors.Add("Tu cuenta no ha sido activada. Por favor, revisa tu correo electrónico para activarla.");
                _logger.LogWarning("Intento de login fallido para el usuario: {UserName}. Cuenta inactiva.", loginDto.UserName);
                return response;
            }

            // 3. Intentar el inicio de sesión
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.Add("Credenciales inválidas. Usuario o contraseña incorrectos.");
                _logger.LogWarning("Intento de login fallido para el usuario: {UserName}. Contraseña incorrecta.", loginDto.UserName);
                return response;
            }

            // 4. Si el login es exitoso, construir el DTO de respuesta
            response.Id = user.Id.ToString();
            response.Name = user.Nombre;
            response.LastName = user.Apellido;
            response.Email = user.Email;
            response.UserName = user.UserName;
            response.IsVerified = user.EstaActivo; // O user.EmailConfirmed, si usas la propiedad de IdentityUser

            // Obtener roles (opcional, si los necesitas en el DTO de respuesta)
            var roles = await _userManager.GetRolesAsync(user);
            response.Roles = roles.ToList();

            _logger.LogInformation("Usuario {UserName} inició sesión exitosamente.", loginDto.UserName);
            return response;
        }

        // Tu LoginAsync actual (SignInResult), que podría ser llamado por un controlador MVC directamente
        // o por AuthenticateAsync si quisieras un resultado más crudo de Identity.
        // Lo mantendremos así por ahora.
        public async Task<SignInResult> LoginAsync(LoginViewModel vm)
        {
            // Aquí puedes añadir la verificación de EstaActivo antes de llamar a PasswordSignInAsync
            var user = await _userManager.FindByNameAsync(vm.Username);
            if (user != null && !user.EstaActivo)
            {
                // Si la cuenta no está activa, no permitas el inicio de sesión
                // Esto NO devuelve un SignInResult que indique inactivo. Tendrías que manejar esto en el controlador
                // de otra manera, o hacer que AuthenticateAsync sea el punto principal de entrada.
                _logger.LogWarning("Intento de login para {UserName} fallido: cuenta inactiva.", vm.Username);
                return SignInResult.NotAllowed; // O un tipo de SignInResult personalizado si tienes uno.
                                                // NotAllowed es lo más cercano para indicar que no se permite iniciar sesión.
            }

            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, lockoutOnFailure: false);
            _logger.LogInformation("Intento de login para {UserName}. Resultado: {Result}", vm.Username, result.ToString());
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            // Aquí la conversión de int a string para el UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
            }
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        }




        public async Task<bool> CheckPasswordAsync(UsuarioViewModel user, string password)
        { 
            // Usa el Id del ViewModel para buscar el ApplicationUser real
            var applicationUser = await _userManager.FindByIdAsync(user.Id.ToString()); // Convierte int a string
            if (applicationUser == null)
            {
                return false; // Usuario no encontrado
            }
            // Usa SignInManager para verificar la contraseña
            var result = await _signInManager.CheckPasswordSignInAsync(applicationUser, password, lockoutOnFailure: false);
            return result.Succeeded;
        }







        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Usuario con ID '{userId}' no encontrado." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                // Si el email es confirmado, activa la cuenta
                if (!user.EstaActivo)
                {
                    user.EstaActivo = true;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError("Fallo al activar la cuenta del usuario {UserName} ({UserId}) después de la confirmación de email: {Errors}", user.UserName, user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                        // Podrías considerar agregar este error a los resultados si quieres reportarlo
                        // result.Errors.Append(new IdentityError { Description = "La cuenta fue confirmada, pero no se pudo activar." });
                    }
                    else
                    {
                        _logger.LogInformation("Cuenta del usuario {UserName} ({UserId}) activada después de la confirmación de email.", user.UserName, user.Id);
                    }
                }
                else
                {
                    _logger.LogInformation("Email del usuario {UserName} ({UserId}) confirmado. La cuenta ya estaba activa.", user.UserName, user.Id);
                }
            }
            else
            {
                _logger.LogWarning("Fallo al confirmar email para el usuario con ID {UserId}. Errores: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }





        public async Task<IdentityResult> DeleteUserAsync(int userId)
        {
            // Aquí la conversión de int a string para el UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
            }
            return await _userManager.DeleteAsync(user);
        }









        public async Task<IdentityResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // No revelar si el usuario no existe o no está confirmado para evitar enumeración de usuarios
            // Si el usuario no existe o no está confirmado, devuelve éxito pero no hagas nada.
            // O si decides, puedes devolver un IdentityResult.Failed con un mensaje genérico.
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) // O !user.EstaActivo si lo usas en vez de EmailConfirmed
            {
                _logger.LogWarning("Intento de ForgotPassword para email {Email} fallido. Usuario no encontrado o email no confirmado.", email);
                return IdentityResult.Success; // Para evitar enumeración de usuarios
            }

            // DESACTIVAR LA CUENTA TEMPORALMENTE (si ese es el requisito exacto)
            // El documento dice: "si el nombre de usuario existe se debe buscar ese usuario y desactivarlo"
            // Esto es un enfoque un poco inusual en la recuperación de contraseña, ya que típicamente no se desactiva la cuenta.
            // Si tu requisito es 'EstaActivo', aquí lo haríamos:
            if (user.EstaActivo) // Solo si está activa, la desactivamos
            {
                user.EstaActivo = false;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Fallo al desactivar la cuenta del usuario {UserName} ({UserId}) durante ForgotPassword: {Errors}", user.UserName, user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    return IdentityResult.Failed(new IdentityError { Description = "Error al procesar la solicitud de restablecimiento de contraseña." });
                }
                _logger.LogInformation("Cuenta del usuario {UserName} ({UserId}) desactivada temporalmente para restablecimiento de contraseña.", user.UserName, user.Id);
            }


            // 1. Generar el token de restablecimiento de contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 2. Enviar el correo con el enlace de restablecimiento
            // Similar a Register, el 'origin' debe venir del exterior o configuración.
            string resetPasswordLink = "https://yourdomain.com/Account/ResetPassword?userId=" + user.Id + "&token=" + Uri.EscapeDataString(token); // O '/Home/ResetPassword' si es MVC

            var emailRequest = new EmailRequestDto
            {
                To = user.Email,
                Subject = "Restablece tu contraseña - BookFace",
                HtmlBody = $"Has solicitado restablecer tu contraseña. Por favor, haz clic en este enlace para continuar: <a href='{resetPasswordLink}'>Restablecer Contraseña</a>" +
                           $"<p>Si no has solicitado esto, puedes ignorar este email.</p>"
            };

            try
            {
                await _emailService.SendAsync(emailRequest);
                _logger.LogInformation("Email de restablecimiento de contraseña enviado a {Email}.", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de restablecimiento a {Email}: {ExceptionMessage}", user.Email, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = "Error al enviar el email de restablecimiento de contraseña." });
            }

            return IdentityResult.Success;
        }








        public async Task<UserDetailsDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return _mapper.Map<UserDetailsDto>(user);
        }








        public async Task<UserDetailsDto?> GetUserByIdAsync(int userId)
        {
            // Aquí la conversión de int a string para el UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return _mapper.Map<UserDetailsDto>(user);
        }










        public async Task<UserDetailsDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return _mapper.Map<UserDetailsDto>(user);
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

      

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }







        // Dentro de la clase IdentityService
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel vm, string origin) // 'origin' es la base del URL desde el controlador
        {
            var user = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Email,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                FechaRegistro = DateTime.UtcNow,
                EstaActivo = false, // El usuario se crea inactivo
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {UserName} registrado exitosamente. Generando token de confirmación de email.", user.UserName);

                // *** ESTAS DOS LÍNEAS SON CRUCIALES Y DEBEN ESTAR PRESENTES ***
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Asegurase de que '/LogIn/ConfirmEmail' sea la ruta URL correcta de tu Razor Page.
                // Basado en tu namespace BookFace.Pages.LogIn, '/LogIn/ConfirmEmail' es lo más probable.
                string confirmationLink = $"{origin}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

                var emailRequest = new EmailRequestDto
                {
                    To = user.Email,
                    Subject = "Confirma tu cuenta - BookFace",
                    HtmlBody = $"Por favor, confirma tu cuenta haciendo clic en este enlace: <a href='{confirmationLink}'>Activar Cuenta</a>"
                };

                _logger.LogInformation("Intentando enviar correo de confirmación a {Email} con link: {Link}", user.Email, confirmationLink);

                try
                {
                    await _emailService.SendAsync(emailRequest);
                    _logger.LogInformation("Correo de confirmación enviado a {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar email de confirmación a {Email}: {ExceptionMessage}", user.Email, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Fallo en el registro del usuario {UserName}. Errores: {Errors}", vm.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return result;
        }











        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel vm)
        {
            // 1. Necesitamos el userId como string para UserManager
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
            {
                // No revelar si el usuario no existe
                return IdentityResult.Failed(new IdentityError { Description = "La solicitud de restablecimiento de contraseña no es válida." });
            }

            // 2. Resetear la contraseña
            var result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword);

            if (result.Succeeded)
            {
                // 3. Si el restablecimiento es exitoso, volver a activar la cuenta
                // Si usas EmailConfirmed, también podrías configurarlo a true si se resetea por primera vez.
                // Pero el requisito es sobre 'EstaActivo'.
                if (!user.EstaActivo) // Si estaba inactiva, activarla
                {
                    user.EstaActivo = true;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError("Fallo al reactivar la cuenta del usuario {UserName} ({UserId}) después del restablecimiento de contraseña: {Errors}", user.UserName, user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                        // Podrías devolver un error aquí si la activación es crítica,
                        // o loguear y asumir que la contraseña fue cambiada pero la cuenta sigue inactiva.
                        // Para el requisito, asumiremos que si la contraseña se cambia, la cuenta debe activarse.
                        return IdentityResult.Failed(new IdentityError { Description = "Contraseña restablecida, pero no se pudo activar la cuenta. Contacte al soporte." });
                    }
                    _logger.LogInformation("Cuenta del usuario {UserName} ({UserId}) reactivada después del restablecimiento de contraseña.", user.UserName, user.Id);
                }
                _logger.LogInformation("Contraseña restablecida exitosamente para el usuario {UserName}.", user.UserName);
            }
            else
            {
                _logger.LogWarning("Fallo al restablecer la contraseña para el usuario con ID {UserId}. Errores: {Errors}", vm.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                // Podrías añadir un mensaje más genérico al usuario si no quieres revelar detalles del token
                // result.Errors.Add(new IdentityError { Description = "Token de restablecimiento inválido o caducado." });
            }

            return result;
        }

        public async Task<IdentityResult> UpdateIdentityUserPropertiesAsync(UsuarioEditViewModel vm)
        {
            // Convierte int userId de vm.Id a string para UserManager.
            // O, si UsuarioEditViewModel.Id ya es string, solo úsalo.
            // Asumiendo que UsuarioEditViewModel.Id es string, según tu última corrección.
            var user = await _userManager.FindByIdAsync(vm.Id.ToString()); // Si vm.Id es int, sería vm.Id.ToString()
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
            }

            // Actualizar solo las propiedades que pueden ser actualizadas directamente
            user.Nombre = vm.Nombre;
            user.Apellido = vm.Apellido;
            user.Email = vm.Email;
            user.UserName = vm.NombreUsuario; // Mapea NombreUsuario del VM a UserName de ApplicationUser
            user.FotoPerfilUrl = vm.FotoPerfilUrl;
            user.PhoneNumber = vm.Telefono;

            // Antes de Update, normaliza UserName y Email
            user.NormalizedUserName = _userManager.NormalizeName(user.UserName);
            user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);


            var result = await _userManager.UpdateAsync(user);

            // Manejo del cambio de contraseña si se proporcionó
            if (result.Succeeded && !string.IsNullOrEmpty(vm.NuevaContrasena))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, vm.ContrasenaActual, vm.NuevaContrasena);
                if (!changePasswordResult.Succeeded)
                {
                    // Si falla el cambio de contraseña, podrías revertir la actualización o registrar el error
                    return changePasswordResult; // Devuelve el error del cambio de contraseña
                }
            }

            return result;
        }



        // Dentro de tu clase IdentityService, añade esta implementación
        public async Task<List<UserDetailsDto>> SearchUsersAsync(string searchTerm, int currentUserId)
        {
            // Obtén todos los usuarios y conviértelos a una lista.
            // Considera paginación si hay muchísimos usuarios.
            var usersQuery = _userManager.Users
                                         .Where(u => u.Id != currentUserId); // Excluir al usuario actual

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.UserName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Nombre.ToLower().Contains(searchTerm) ||
                    u.Apellido.ToLower().Contains(searchTerm)
                );
            }

            var users = await usersQuery.ToListAsync(); // Asegúrate de tener 'using Microsoft.EntityFrameworkCore;' para ToListAsync
            return _mapper.Map<List<UserDetailsDto>>(users);
        }
    }
}
