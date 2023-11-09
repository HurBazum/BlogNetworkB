using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Author
{
    public class UpdateAuthorRequest
    {
        public string? NewFirstName { get; set; }
        public string? NewLastName { get; set; }
        public string? NewLogin { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче 6 символов")]
        public string? NewPassword { get; set; }

        [EmailAddress]
        public string? NewEmail { get; set; }
    }
}