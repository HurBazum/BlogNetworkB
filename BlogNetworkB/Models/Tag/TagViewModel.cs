using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Tag
{
    public class TagViewModel
    {
        [Required]
        // ???
        [MaxLength(25, ErrorMessage = "Тег должен быть не длиннее 25 символов")]
        [DataType(DataType.Text)]
        public string Content { get; set; } = null!;
    }
}