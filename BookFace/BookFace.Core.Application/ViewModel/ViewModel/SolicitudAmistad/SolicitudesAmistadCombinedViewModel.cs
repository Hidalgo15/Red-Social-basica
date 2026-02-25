

namespace BookFace.Core.Application.ViewModel.ViewModel.SolicitudAmistad
{
    public class SolicitudesAmistadCombinedViewModel
    {

        public List<SolicitudAmistadViewModel> ReceivedRequests { get; set; } = new List<SolicitudAmistadViewModel>();
        public List<SolicitudAmistadViewModel> SentRequests { get; set; } = new List<SolicitudAmistadViewModel>();
    }
}
