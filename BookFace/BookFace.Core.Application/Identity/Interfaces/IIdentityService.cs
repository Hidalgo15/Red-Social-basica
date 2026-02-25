using BookFace.Core.Application.DTO;
using BookFace.Core.Application.ViewModel.ViewModel.Auth;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using Microsoft.AspNetCore.Identity;

namespace BookFace.Core.Application.Identity.Interfaces
{
    public interface IIdentityService
    {
        // Métodos de Autenticación
        Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel vm , string callbackUrl);
        Task<SignInResult> LoginAsync(LoginViewModel vm);
        Task LogoutAsync();

        // Métodos de Recuperación/Restablecimiento de Contraseña
        Task<IdentityResult> ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel vm);

        // Métodos de Confirmación de Email
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        // Métodos para Obtener Información del Usuario
        Task<UserDetailsDto?> GetUserByIdAsync(int userId); // userId debe ser string
        Task<UserDetailsDto?> GetUserByUsernameAsync(string username);
        Task<UserDetailsDto?> GetUserByEmailAsync(string email);

        // Métodos para Actualizar Propiedades de Identity
        Task<IdentityResult> UpdateIdentityUserPropertiesAsync(UsuarioEditViewModel vm);
        Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        // Métodos de Utilidad
        Task<bool> CheckPasswordAsync(UsuarioViewModel user, string password); // Para validar contraseña de un Domain.Usuario
        string? GetUserId(); // Obtiene el ID del usuario logueado desde el HttpContext
        string? GetUserName(); // Obtiene el UserName del usuario logueado desde el HttpContext
        Task<IdentityResult> DeleteUserAsync(int userId); // Para eliminar un usuario de Identity


        // Añade esto a tu interfaz IIdentityService
        Task<List<UserDetailsDto>> SearchUsersAsync(string searchTerm, int currentUserId);
    }
}
