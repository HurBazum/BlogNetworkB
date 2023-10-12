using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Role
{
    public class RoleViewModel
    {
        public int AuthorId { get; set; }

        [Required]
        [Display(Name = "Введите id роли")]
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}