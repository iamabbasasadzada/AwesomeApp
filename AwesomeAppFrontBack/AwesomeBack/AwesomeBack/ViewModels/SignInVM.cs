using System.ComponentModel.DataAnnotations;

namespace AwesomeBack.ViewModels
{
    public class SignInVM
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required , DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
