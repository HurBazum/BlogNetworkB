﻿using System.ComponentModel.DataAnnotations;

namespace BlogNetworkB.Models.Article
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }
        public int CreatorId { get; set; }
               

        [Required]
        [MinLength(5, ErrorMessage = "Минимальная длина заголовка - 5 символов")]
        [MaxLength(40, ErrorMessage = "Максимальная длина заголовка - 40 символов")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [MinLength(15, ErrorMessage = "Минимальная длина статьи - 15 символов")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "Несколько слов")]
        public string Description => (Content.Length > 200) ? string.Concat(Content[..199], "...") : Content;
        public DateTime CreatedDate { get; set; }

        public Dictionary<string, bool> ArticleTags { get; set; } = new();
    }
}