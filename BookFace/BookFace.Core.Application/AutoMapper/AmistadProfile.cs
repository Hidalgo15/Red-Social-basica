using AutoMapper;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using BookFace.Core.Application.ViewModel.ViewModel.Amistad;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Application.AutoMapper
{
    public class AmistadProfile : Profile
    {
        public AmistadProfile()
        {
            /* // Mapeo de Entidad Amistad a AmistadViewModel (para mostrar)
             // AutoMapper se encargará de los mapeos anidados de Usuario1 y Usuario2
             CreateMap<Amistad, AmistadViewModel>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Asegúrate de mapear el Id
                 .ForMember(dest => dest.Usuario1, opt => opt.MapFrom(src => src.UsuarioId1))
                 .ForMember(dest => dest.Usuario2, opt => opt.MapFrom(src => src.UsuarioId2))
                 .ForMember(dest => dest.Usuario1.Nombre, opt => opt.Ignore()) // Se llenará en el servicio
                 .ForMember(dest => dest.Usuario1.FotoPerfilUrl, opt => opt.Ignore()) // Se llenará en el servicio
                 .ForMember(dest => dest.Usuario2.Nombre, opt => opt.Ignore()) // Se llenará en el servicio
                 .ForMember(dest => dest.Usuario2.FotoPerfilUrl, opt => opt.Ignore()) // Se llenará en el servicio
                 .ForMember(dest => dest.FechaAmistad, opt => opt.MapFrom(src => src.FechaAmistad)); // Asumiendo que Amistad tiene FechaAmistad


             // Mapeo de AmistadSaveViewModel a Entidad Amistad (para crear)
             // Llama al constructor de la entidad Amistad con los IDs del ViewModel
             CreateMap<AmistadSaveViewModel, Amistad>()
                 .ConstructUsing(src => new Amistad(src.UsuarioId1, src.UsuarioId2));
            */

            // Mapeo de Entidad Amistad a AmistadViewModel (para mostrar)
            // AutoMapper se encargará de los mapeos anidados de Usuario1 y Usuario2
            CreateMap<Amistad, AmistadViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Asegúrate de mapear el Id
                .ForMember(dest => dest.Usuario1, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.Usuario2, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.FechaAmistad, opt => opt.MapFrom(src => src.FechaAmistad)); // Asumiendo que Amistad tiene FechaAmistad

            // Mapeo de AmistadSaveViewModel a Entidad Amistad (para crear)
            // Llama al constructor de la entidad Amistad con los IDs del ViewModel
            CreateMap<AmistadSaveViewModel, Amistad>()
                .ConstructUsing(src => new Amistad(src.UsuarioId1, src.UsuarioId2));

        }
    }
}
