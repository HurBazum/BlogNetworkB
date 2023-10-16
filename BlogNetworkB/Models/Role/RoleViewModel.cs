using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Role
{
    public class RoleViewModel
    {
        public int AuthorId { get; set; }

        [Required]
        [Display(Name = "Введите id роли")]
        public int RoleId { get; set; }

        [Required]
        [Display(Name = "Название")]
        [MinLength(2, ErrorMessage = "Слишком короткое название для роли!(Минимум - 2 буквы)")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MinLength(2, ErrorMessage = "Слишком короткое описание для роли!(Минимум - 10 символов)")]
        [DataType(DataType.Text)]
        public string Description { get; set; }
    }
}