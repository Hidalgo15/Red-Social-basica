using BookFace.Core.Domain.Entities;
using AutoMapper;
using BookFace.Core.Application.ViewModel.ViewModel.Reaccion;

namespace BookFace.Core.Application.AutoMapper
{
    public class ReaccionProfile : Profile
    {
        public ReaccionProfile()
        {
            // Mapeos para Reaccion
            CreateMap<Reaccion, ReaccionViewModel>()
                .ForMember(dest => dest.NombreUsuario, opt => opt.Ignore()) // Se llena en el servicio
                .ForMember(dest => dest.FotoPerfilUsuarioUrl, opt => opt.Ignore()) // Se llena en el servicio
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ReaccionSaveViewModel, Reaccion>();
            CreateMap<Reaccion, ReaccionSaveViewModel>(); // Si necesitas mapear la entidad de vuelta a SaveViewModel (para edición, etc.)
        }
    }
}
