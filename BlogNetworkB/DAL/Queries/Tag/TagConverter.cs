namespace BlogNetworkB.DAL.Queries.Tag
{
    public static class TagConverter
    {
        public static Enteties.Tag Convert(Enteties.Tag tag, UpdateTagQuery utq)
        {
            tag.Content = (!string.IsNullOrEmpty(utq.NewContent)) ? utq.NewContent : tag.Content;

            return tag;
        }
    }
}