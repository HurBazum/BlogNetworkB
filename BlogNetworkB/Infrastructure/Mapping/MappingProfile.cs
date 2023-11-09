using AutoMapper;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Comment;
using BlogNetworkB.BLL.Models.Tag;
using ConnectionLib.DAL.Enteties;
using ConnectionLib.DAL.Queries.Article;
using ConnectionLib.DAL.Queries.Author;
using ConnectionLib.DAL.Queries.Comment;
using ConnectionLib.DAL.Queries.Tag;
using BlogNetworkB.Models.Account;
using BlogNetworkB.Models.Article;
using BlogNetworkB.Models.Comment;
using BlogNetworkB.Models.Tag;
using BlogNetworkB.Models.Role;
using BlogNetworkB.BLL.Models.Role;
using ConnectionLib.DAL.Queries.Role;

namespace BlogNetworkB.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // author
            CreateMap<RegisterViewModel, Author>();
            CreateMap<Author, AuthorViewModel>().ForMember("AuthorId", avm => avm.MapFrom(a => a.Id));
            CreateMap<AuthorViewModel, Author>();
            CreateMap<UpdateAuthorRequestViewModel, UpdateAuthorRequest>();
            CreateMap<UpdateAuthorRequest, UpdateAuthorQuery>();

            CreateMap<CreateAuthorO, AuthorDTO>();
            CreateMap<RegisterViewModel, AuthorDTO>();
            CreateMap<Author, AuthorDTO>().ForMember("AuthorId", dto => dto.MapFrom(a => a.Id));
            CreateMap<AuthorDTO, Author>();
            CreateMap<AuthorDTO, AuthorViewModel>();
            CreateMap<AuthorViewModel, AuthorDTO>();

            // article
            CreateMap<ArticleViewModel, Article>();
            CreateMap<Article, ArticleViewModel>()
                .ForMember("CreatorId", avm => avm.MapFrom(a => a.AuthorId))
                .ForMember("ArticleId", avm => avm.MapFrom(a => a.Id));
            CreateMap<UpdateArticleRequestViewModel, UpdateArticleRequest>();
            CreateMap<UpdateArticleRequest, UpdateArticleQuery>();

            CreateMap<Article, ArticleDTO>().ForMember("ArticleId", avm => avm.MapFrom(dto => dto.Id));
            CreateMap<ArticleDTO, Article>();
            CreateMap<CreateArticleO, ArticleDTO>();
            CreateMap<ArticleDTO, ArticleViewModel>()
                .ForMember("CreatorId", avm => avm.MapFrom(dto => dto.AuthorId));
            CreateMap<ArticleViewModel, ArticleDTO>();

            // tag
            CreateMap<TagViewModel, Tag>();
            CreateMap<Tag, TagViewModel>().ForMember("TagId", tvm => tvm.MapFrom(t => t.Id));
            CreateMap<UpdateTagRequestViewModel, UpdateTagRequest>();
            CreateMap<UpdateTagRequest, UpdateTagQuery>();

            CreateMap<Tag, TagDTO>().ForMember("TagId", tvm => tvm.MapFrom(dto => dto.Id));
            CreateMap<TagDTO, Tag>();
            CreateMap<CreateTagO, TagDTO>();
            CreateMap<TagDTO, TagViewModel>();

            // comment
            CreateMap<Comment, CommentViewModel>().ForMember("CommentId", cvm => cvm.MapFrom(c => c.Id));
            CreateMap<CreateCommentViewModel, Comment>();
            CreateMap<UpdateCommentRequestViewModel, UpdateCommentRequest>();
            CreateMap<UpdateCommentRequest, UpdateCommentQuery>();

            CreateMap<Comment, CommentDTO>().ForMember("CommentId", cvm => cvm.MapFrom(dto => dto.Id));
            CreateMap<CommentDTO, Comment>();
            CreateMap<CreateCommentO, CommentDTO>();
            CreateMap<CommentDTO, CommentViewModel>();
            CreateMap<CreateCommentViewModel, CommentDTO>();

            // role
            CreateMap<Role, RoleViewModel>().ForMember("RoleId", rvm => rvm.MapFrom(r => r.Id));
            CreateMap<RoleViewModel, Role>();
            CreateMap<UpdateRoleRequestViewModel, UpdateRoleDescriptionRequest>();
            CreateMap<UpdateRoleDescriptionRequest, UpdateRoleQuery>();
                        
            CreateMap<Role, RoleDTO>().ForMember("RoleId", rvm => rvm.MapFrom(dto => dto.Id));
            CreateMap<RoleDTO, Role>();
            CreateMap<CreateRoleO, RoleDTO>();
            CreateMap<RoleDTO, RoleViewModel>();
            CreateMap<RoleViewModel, RoleDTO>();
        }
    }
}