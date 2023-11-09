﻿using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.BLL.Models.Tag
{
    public class UpdateTagRequest
    {
        [MaxLength(25, ErrorMessage = "Тег должен быть не длиннее 25 символов")]
        [DataType(DataType.Text)]
        public string NewContent { get; set; }
    }
}