using AutoMapper;
using BookFace.Core.Application.DTO;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;


namespace BookFace.Infraestructure.Identity.Entities
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            // Este es el mapeo crucial que AutoMapper no encuentra
            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Convert.ToInt32(src.Id)))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl));

            CreateMap<UserDetailsDto, UsuarioViewModel>()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
       .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
       .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
       .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
       .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl));

            // Mapeo de UserDetailsDto a UsuarioEditViewModel (¡LA SOLUCIÓN A TU PROBLEMA ACTUAL!)
            // Este mapeo es para ProfileController.Edit() GET, donde _identityService.GetUserByIdAsync
            // devuelve UserDetailsDto y necesitas mapearlo a UsuarioEditViewModel para el formulario.
            CreateMap<UserDetailsDto, UsuarioEditViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) // Asegúrate de mapear el email
                .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
                .ForMember(dest => dest.ContrasenaActual, opt => opt.Ignore()) // No mapear contraseñas directas
                .ForMember(dest => dest.NuevaContrasena, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmarContrasena, opt => opt.Ignore());

            // Mapeo de UsuarioEditViewModel a ApplicationUser (para Profile/Edit POST)
            // Este mapeo es si necesitas convertir UsuarioEditViewModel de vuelta a ApplicationUser
            // para la actualización en tu IIdentityService.UpdateIdentityUserPropertiesAsync(vm).
            // Si tu servicio acepta UsuarioEditViewModel directamente, este mapeo es para la capa de Infraestructura
            // o para AutoMapper dentro del servicio si mapeas antes de persistir.
            CreateMap<UsuarioEditViewModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString())) // Asume que ApplicationUser.Id es string
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.NombreUsuario)) // UserName en ApplicationUser
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            // No mapear FotoPerfilUrl aquí directamente ya que se maneja por separado si es un IFormFile.
            // Para contraseñas, tu servicio UpdateIdentityUserPropertiesAsync() debería manejarlas
            // usando UserManager.ChangePasswordAsync o similar.

                    CreateMap<ApplicationUser, UsuarioViewModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Convert.ToInt32(src.Id)))
               .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
               .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
               .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl));

                    CreateMap<UserDetailsDto,UsuarioEditViewModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
             .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
             .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
             .ForMember(dest => dest.ContrasenaActual, opt => opt.Ignore())
             .ForMember(dest => dest.NuevaContrasena, opt => opt.Ignore())
             .ForMember(dest => dest.ConfirmarContrasena, opt => opt.Ignore());

        }
    }
}
