namespace ConnectionLib.DAL.Enteties
{
    public class Tag
    {
        public int Id { get; set; }
        public string Content { get; set; }

        // rel
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}