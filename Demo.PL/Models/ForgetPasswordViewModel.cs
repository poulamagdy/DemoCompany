using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
	public class ForgetPasswordViewModel
	{
		[Required(ErrorMessage = "Email Required")]
		[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
		ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
    }
}
