namespace BlogNetworkB.DAL.Enteties
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // rel
        public ICollection<Author> Authors { get; set; } = new List<Author>();
    }
}