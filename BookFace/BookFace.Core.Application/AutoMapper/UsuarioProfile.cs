using AutoMapper;
using BookFace.Core.Application.DTO;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;


namespace BookFace.Core.Application.AutoMapper
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<UserDetailsDto, UsuarioViewModel>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
          .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
          .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
          .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
          
          .ForMember(dest => dest.CorreoElectronico, opt => opt.Ignore())
          .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
          .ForMember(dest => dest.EstaActivo, opt => opt.Ignore());

        }
    }
}