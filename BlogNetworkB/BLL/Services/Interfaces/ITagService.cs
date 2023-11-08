using BlogNetworkB.BLL.Models.Tag;

namespace BlogNetworkB.BLL.Services.Interfaces
{
    public interface ITagService
    {
        public Task<TagDTO[]> GetAllTagsDTOs();
        public Task<List<TagDTO>> GetTagsDTOsByContent(params string[] content);
        public Task AddTag(TagDTO tagDTO);
        public Task DeleteTag(int id);
        public Task<TagDTO> GetTagDTOById(int id);
        public Task<bool> SearchSameTag(string content);
        public Task UpdateTag(TagDTO tagDTO, UpdateTagRequest utr);
    }
}
