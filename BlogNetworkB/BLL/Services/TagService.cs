using AutoMapper;
using BlogNetworkB.BLL.Models.Tag;
using BlogNetworkB.BLL.Services.Interfaces;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Tag;

namespace BlogNetworkB.BLL.Services
{
    public class TagService : ITagService
    {
        readonly ITagRepository _tagRepository;
        readonly IMapper _mapper;

        public TagService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task AddTag(TagDTO tagDTO)
        {
            var tag = _mapper.Map<Tag>(tagDTO);

            await _tagRepository.AddTag(tag);
        }

        public async Task<TagDTO[]> GetAllTagsDTOs()
        {
            var tags = await _tagRepository.GetAll();

            return _mapper.Map<TagDTO[]>(tags);
        }

        public async Task<List<TagDTO>> GetTagsDTOsByContent(params string[] contents)
        {
            List<TagDTO> tagDTOs = new();

            for(int i = 0; i < contents.Length; i++)
            {
                tagDTOs.Add(_mapper.Map<TagDTO>(await _tagRepository.GetTagByContent(contents[i])));
            }

            return tagDTOs;
        }

        public async Task DeleteTag(int id)
        {
            var tag = await _tagRepository.GetTagById(id);

            await _tagRepository.DeleteTag(tag);
        }

        public async Task<TagDTO> GetTagDTOById(int id)
        {
            var tag = await _tagRepository.GetTagById(id);
            
            return _mapper.Map<TagDTO>(tag);
        }

        public async Task<bool> SearchSameTag(string content)
        {
            if(await _tagRepository.GetTagByContent(content) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task UpdateTag(TagDTO tagDTO, UpdateTagRequest utr)
        {
            var tag = await _tagRepository.GetTagById(tagDTO.TagId);

            await _tagRepository.UpdateTag(tag, _mapper.Map<UpdateTagQuery>(utr));
        }
    }
}