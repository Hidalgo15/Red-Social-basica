using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad
{
    public class SolicitudAmistadViewModel
    {
        public int Id { get; set; }
        public int RemitenteId { get; set; } // Añadir el ID del remitente
        public string NombreRemitente { get; set; } // Este campo se llenará en el servicio
        public string FotoPerfilRemitenteUrl { get; set; } // Añadir URL de la foto del remitente
        public int ReceptorId { get; set; } // Añadir el ID del receptor
        public string NombreReceptor { get; set; } // Este campo se llenará en el servicio
        public string FotoPerfilReceptorUrl { get; set; } // Añadir URL de la foto del receptor
        public EstadoSolicitudAmistad Estado { get; set; } // Mapea el estado de la solicitud [cite: 258]
        public DateTime FechaSolicitud { get; set; } // Fecha en que se realizó la solicitud [cite: 249]
        public int CantidadAmigosComunes { get; set; } // Cantidad de amigos comunes [cite: 249, 258]

    }
}
