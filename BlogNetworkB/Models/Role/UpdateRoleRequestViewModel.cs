using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Role
{
    public class UpdateRoleRequestViewModel
    {
        public int RoleId { get; set; }

        [Required]
        [Display(Name = "Новое имя")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "Новое описание")]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Слишком короткое описание роли!(Минимум 10 символов)")]
        public string NewDescription { get; set; }
    }
}