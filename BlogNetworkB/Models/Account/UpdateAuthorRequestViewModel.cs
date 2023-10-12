using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Account
{
    public class UpdateAuthorRequestViewModel
    {
        public int? AuthorId { get; set; }

        [Display(Name = "Новое имя")]
        public string? NewFirstName { get; set; }

        [Display(Name = "Новая фамилия")]
        public string? NewLastName { get; set; }

        [Display(Name = "Новый логин")]
        public string? NewLogin { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль не должен быть короче 6 символов")]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }

        [EmailAddress]
        [Display(Name = "Новый email")]
        public string? NewEmail { get; set; }
    }
}
