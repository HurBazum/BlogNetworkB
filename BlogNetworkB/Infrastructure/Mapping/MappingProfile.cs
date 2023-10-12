using AutoMapper;
using BlogNetworkB.BLL.Models.Article;
using BlogNetworkB.BLL.Models.Author;
using BlogNetworkB.BLL.Models.Comment;
using BlogNetworkB.BLL.Models.Tag;
using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Queries.Article;
using BlogNetworkB.DAL.Queries.Author;
using BlogNetworkB.DAL.Queries.Comment;
using BlogNetworkB.DAL.Queries.Tag;
using BlogNetworkB.Models.Account;
using BlogNetworkB.Models.Article;
using BlogNetworkB.Models.Comment;
using BlogNetworkB.Models.Tag;
using BlogNetworkB.Models.Role;

namespace BlogNetworkB.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateAuthorRequestViewModel, UpdateAuthorRequest>();
            CreateMap<UpdateAuthorRequest, UpdateAuthorQuery>();

            CreateMap<UpdateCommentRequest, UpdateCommentQuery>();

            CreateMap<UpdateArticleRequest, UpdateArticleQuery>();

            CreateMap<UpdateTagRequest, UpdateTagQuery>();

            CreateMap<RegisterViewModel, Author>();
            CreateMap<ArticleViewModel, Article>();
            CreateMap<Article, ArticleViewModel>().ForMember("CreatorId", avm => avm.MapFrom(a => a.AuthorId));
            CreateMap<TagViewModel, Tag>();
            CreateMap<Tag, TagViewModel>();
            CreateMap<Author, AuthorViewModel>().ForMember("AuthorId", avm => avm.MapFrom(a => a.Id));
            CreateMap<AuthorViewModel, Author>();
            CreateMap<Comment, CommentViewModel>();
            CreateMap<CreateCommentViewModel, Comment>();
            CreateMap<Role, RoleViewModel>().ForMember("RoleId", rvm => rvm.MapFrom(r => r.Id));
        }
    }
}