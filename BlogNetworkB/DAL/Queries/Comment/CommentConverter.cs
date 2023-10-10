namespace BlogNetworkB.DAL.Queries.Comment
{
    public static class CommentConverter
    {
        public static string Convert(Enteties.Comment c,
            UpdateCommentQuery ucq) => c.Content = ucq.NewContent;
    }
}
