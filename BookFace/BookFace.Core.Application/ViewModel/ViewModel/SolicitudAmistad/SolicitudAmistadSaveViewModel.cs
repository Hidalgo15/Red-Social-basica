using System.ComponentModel.DataAnnotations;

namespace BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad
{
    public class SolicitudAmistadSaveViewModel
    {

        [Required]
        public int RemitenteId { get; set; }
        [Required]
        public int ReceptorId { get; set; }
    }
}
