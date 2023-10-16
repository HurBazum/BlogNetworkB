namespace ConnectionLib.DAL.Queries.Article
{
    public static class ArticleConverter
    {
        public static Enteties.Article Convert(Enteties.Article a, UpdateArticleQuery uaq)
        {
            a.Title = (!string.IsNullOrEmpty(uaq.NewTitle)) ? uaq.NewTitle : a.Title;
            a.Content = (!string.IsNullOrEmpty(uaq.NewContent)) ? uaq.NewContent : a.Content;

            return a;
        }
    }
}