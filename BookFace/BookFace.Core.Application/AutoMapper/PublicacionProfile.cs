using AutoMapper;
using BookFace.Core.Application.DTO;
using BookFace.Core.Application.ViewModel.ViewModel.Publicacion;
using BookFace.Core.Application.ViewModel.ViewModel.Usuario;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Application.AutoMapper
{
    public class PublicacionProfile : Profile
    {
        public PublicacionProfile()
        {
            // Mapeo de Entidad Publicacion a PublicacionViewModel (para mostrar)
            CreateMap<Publicacion, PublicacionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.ImagenUrl))
                .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.VideoUrl))
                // .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes)) // ELIMINAR O IGNORAR: PublicacionViewModel ya no tiene 'Likes' int.
                // Ahora tiene CantidadMeGusta y CantidadNoMeGusta, que se calculan.
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))

                // --- CAMBIO 1: Corregir mapeo de Usuario ---
                // Asumiendo que Publicacion.UsuarioId es un int que referencia el ID del usuario.
                // Creamos un nuevo UsuarioViewModel y le asignamos solo el Id.
                // El Nombre y FotoPerfilUrl se llenarán en el PublicacionService.
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => new UsuarioViewModel { Id = src.UsuarioId }))
             //   .ForMember(dest => dest.Usuario.Nombre, opt => opt.Ignore()) // Se llenará en el servicio
             //   .ForMember(dest => dest.Usuario.FotoPerfilUrl, opt => opt.Ignore()) // Se llenará en el servicio

                // Si tu entidad Publicacion tiene una propiedad de navegación 'Usuario' (ej: public Usuario Usuario { get; set; })
                // Y tienes un mapeo de 'Usuario' (entidad) a 'UsuarioViewModel', entonces puedes usar:
                // .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
                // Y aún ignorar Nombre/FotoPerfilUrl si los enriqueces en el servicio.


                // --- CAMBIO 2: Ignorar CantidadMeGusta y CantidadNoMeGusta ---
                // Estas propiedades se calcularán en el PublicacionService, no se mapean directamente desde la entidad.
                .ForMember(dest => dest.CantidadMeGusta, opt => opt.Ignore())
                .ForMember(dest => dest.CantidadNoMeGusta, opt => opt.Ignore())

                // --- CAMBIO 3: Mapear la colección de Reacciones ---
                // AutoMapper mapeará la colección 'Reacciones' de la entidad a 'Reacciones' del ViewModel.
                // Asegúrate de que Reaccion (entidad) -> ReaccionViewModel (mapeo de perfil) exista.
                .ForMember(dest => dest.Reacciones, opt => opt.MapFrom(src => src.Reacciones))

                // Mapea la colección de comentarios
                .ForMember(dest => dest.Comentarios, opt => opt.MapFrom(src => src.Comentarios));


            // Mapeo de PublicacionSaveViewModel a Entidad Publicacion (para crear)
            CreateMap<PublicacionSaveViewModel, Publicacion>()
                .ConstructUsing(src => new Publicacion(
                    src.Contenido,
                    src.UsuarioId,
                    src.ImagenUrl,
                    src.VideoUrl
                ));

            // Mapeo de PublicacionEdicionViewModel a Entidad Publicacion (para actualizar)
            CreateMap<PublicacionEditViewModel, Publicacion>()
                 .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Contenido))
                 .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.ImagenUrl))
                 .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.VideoUrl))
                 .ForMember(dest => dest.Id, opt => opt.Ignore()) // El Id ya existe en la entidad, no se mapea desde el VM
                 .ForMember(dest => dest.UsuarioId, opt => opt.Ignore()) // UsuarioId no se edita a través de este VM
                 .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Fecha de creación no se edita
                                                                             // .ForMember(dest => dest.Likes, opt => opt.Ignore()) // ELIMINAR O IGNORAR: PublicacionEditViewModel no tiene 'Likes' int.
                 .ForMember(dest => dest.Comentarios, opt => opt.Ignore()) // Ignorar colección de navegación en la edición, ya que no se actualiza directamente así
                                                                           // --- CAMBIO 4: Ignorar Reacciones en PublicacionEditViewModel ---
                 .ForMember(dest => dest.Reacciones, opt => opt.Ignore());

            // Mapeo de Entidad Publicacion a PublicacionSaveViewModel (para el retorno del método Add del GenericService)
            CreateMap<Publicacion, PublicacionSaveViewModel>()
                .ForMember(dest => dest.Contenido, opt => opt.MapFrom(src => src.Contenido))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.ImagenUrl))
                .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.VideoUrl))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId));
            // No necesitas mapear Id aquí porque PublicacionSaveViewModel no tiene propiedad Id.
            // Si tu PublicacionSaveViewModel tuviera un Id para retornar, lo mapearías así:
            // .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserDetailsDto, UsuarioViewModel>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}")) // Combina nombre y apellido si tu ViewModel solo tiene "Nombre"
           .ForMember(dest => dest.FotoPerfilUrl, opt => opt.MapFrom(src => src.FotoPerfilUrl))
           .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario)); // O mapea UserName del DTO

        }
    }
}
