using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Tag;

namespace ConnectionLib.DAL.Repositories.Interfaces
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