using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Role
{
    public class UpdateRoleDescriptionRequest
    {
        [MinLength(2, ErrorMessage = "Слишком короткое описание для роли!(Минимум - 10 символов)")]
        [DataType(DataType.Text)]
        public string NewDescription { get; set; }
    }
}