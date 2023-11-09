using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Role
{
    public class CreateRoleO
    {
        [Required]
        [MinLength(2, ErrorMessage = "Слишком короткое название для роли!(Минимум - 2 буквы)")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}