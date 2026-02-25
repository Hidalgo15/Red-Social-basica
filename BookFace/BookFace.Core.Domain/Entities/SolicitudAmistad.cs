using BookFace.Core.Domain.Enums;

namespace BookFace.Core.Domain.Entities
{
    public class SolicitudAmistad
    {
        public int Id { get; private set; }
        public int RemitenteId { get; private set; } // ID del usuario que envía la solicitud
        public int ReceptorId { get; private set; } // ID del usuario que recibe la solicitud
        public DateTime FechaSolicitud { get;  set; }
        public EstadoSolicitudAmistad Estado { get;  set; }
        

        // Propiedades de navegación no se pueden usar porque usuario ya no existe

        // public Usuario Remitente { get; private set; }
        // public Usuario Receptor { get; private set; }

        // Constructor
        public SolicitudAmistad(int remitenteId, int receptorId)
        {
            if (remitenteId <= 0 || receptorId <= 0) throw new ArgumentException("Los IDs de usuario deben ser válidos.");
            if (remitenteId == receptorId) throw new ArgumentException("Un usuario no puede enviarse una solicitud de amistad a sí mismo.");

            RemitenteId = remitenteId;
            ReceptorId = receptorId;
            FechaSolicitud = DateTime.UtcNow;
            Estado = EstadoSolicitudAmistad.Pendiente; // Por defecto, una nueva solicitud está pendiente
        }

        // Constructor vacío para EF Core
        protected SolicitudAmistad() { }

        // Métodos de comportamiento de dominio
        public void Aceptar()
        {
            if (Estado != EstadoSolicitudAmistad.Pendiente)
            {
                throw new InvalidOperationException("La solicitud no se puede aceptar porque no está pendiente.");
            }
            Estado = EstadoSolicitudAmistad.Aceptada;
        }

        public void Rechazar()
        {
            if (Estado != EstadoSolicitudAmistad.Pendiente)
            {
                throw new InvalidOperationException("La solicitud no se puede rechazar porque no está pendiente.");
            }
            Estado = EstadoSolicitudAmistad.Rechazada;
        }
    }
}
