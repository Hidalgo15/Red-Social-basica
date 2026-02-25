using AutoMapper;
using BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad;
using BookFace.Core.Domain.Entities;

namespace BookFace.Core.Application.AutoMapper
{
    public class SolicitudAmistadProfile : Profile
    { 
        public SolicitudAmistadProfile()
        {
            // Mapeo de Entidad SolicitudAmistad a SolicitudAmistadViewModel
            CreateMap<SolicitudAmistad, SolicitudAmistadViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Asegúrate de mapear el Id
                .ForMember(dest => dest.RemitenteId, opt => opt.MapFrom(src => src.RemitenteId))
                .ForMember(dest => dest.ReceptorId, opt => opt.MapFrom(src => src.ReceptorId))
                .ForMember(dest => dest.NombreRemitente, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.FotoPerfilRemitenteUrl, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.NombreReceptor, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.FotoPerfilReceptorUrl, opt => opt.Ignore()) // Se llenará en el servicio
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.FechaSolicitud, opt => opt.MapFrom(src => src.FechaSolicitud))
                .ForMember(dest => dest.CantidadAmigosComunes, opt => opt.Ignore()); // Se llenará en el servicio

            // Mapeo de SolicitudAmistadSaveViewModel a Entidad SolicitudAmistad
            CreateMap<SolicitudAmistadSaveViewModel, SolicitudAmistad>();
        }
    }
}
