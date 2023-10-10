using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Queries.Tag;

namespace BlogNetworkB.DAL.Repositories.Interfaces
{
    public interface ITagRepository
    {
        public Task AddTag(Tag tag);
        public Task UpdateTag(Tag tag, UpdateTagQuery updateTagQuery);
        public Task DeleteTag(Tag tag);
        public Task<Tag> GetTagById(int id);
        public Task<Tag[]> GetAll();
        public Task<Tag> GetTagByContent(string content);
    }
}