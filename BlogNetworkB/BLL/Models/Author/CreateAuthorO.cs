using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Author
{
    public class CreateAuthorO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче 6 символов")]
        public string Password { get; set; }
    }
}