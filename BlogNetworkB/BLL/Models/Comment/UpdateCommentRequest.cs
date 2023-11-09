﻿using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Comment
{
    public class UpdateCommentRequest
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "Слишком короткий комментарий")]
        public string NewContent { get; set; } = null!;
    }
}