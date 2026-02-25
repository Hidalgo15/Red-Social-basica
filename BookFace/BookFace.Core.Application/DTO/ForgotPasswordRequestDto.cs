namespace BookFace.Core.Application.DTO
{
    public class ForgotPasswordRequestDto
    {
        public required string UserName { get; set; }
        public required string Origin { get; set; }
    }
}
