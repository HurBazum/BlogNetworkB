using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Tag
{
    public class UpdateTagRequestViewModel
    {
        public int TagId { get; set; }

        [MaxLength(25, ErrorMessage = "Тег должен быть не длиннее 25 символов")]
        [DataType(DataType.Text)]
        public string NewContent { get; set; } = null!;
    }
}