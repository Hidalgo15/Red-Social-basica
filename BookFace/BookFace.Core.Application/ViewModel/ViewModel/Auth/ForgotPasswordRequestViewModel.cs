using System.ComponentModel.DataAnnotations;


namespace BookFace.Core.Application.ViewModel.ViewModel.Auth
{
    public class ForgotPasswordRequestViewModel
    {

        [Required(ErrorMessage = "You must enter the username of user")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }
    }
}
