using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; } = null!;

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче 6 символов")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare(otherProperty: "Password", ErrorMessage = "Пароли не совпадают!")]
        public string ConfirmPassword { get; set; } = null!;

    }
}