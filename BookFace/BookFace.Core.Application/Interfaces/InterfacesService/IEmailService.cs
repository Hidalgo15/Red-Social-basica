using BookFace.Core.Application.DTO;

namespace BookFace.Core.Application.Interfaces.InterfacesService
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}
