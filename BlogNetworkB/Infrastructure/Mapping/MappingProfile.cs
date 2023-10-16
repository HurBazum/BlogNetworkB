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

            // article
            CreateMap<ArticleViewModel, Article>();
            CreateMap<Article, ArticleViewModel>()
                .ForMember("CreatorId", avm => avm.MapFrom(a => a.AuthorId))
                .ForMember("ArticleId", avm => avm.MapFrom(a => a.Id));
            CreateMap<UpdateArticleRequestViewModel, UpdateArticleRequest>();
            CreateMap<UpdateArticleRequest, UpdateArticleQuery>();

            // tag
            CreateMap<TagViewModel, Tag>();
            CreateMap<Tag, TagViewModel>().ForMember("TagId", tvm => tvm.MapFrom(t => t.Id));
            CreateMap<UpdateTagRequestViewModel, UpdateTagRequest>();
            CreateMap<UpdateTagRequest, UpdateTagQuery>();

            // comment
            CreateMap<Comment, CommentViewModel>().ForMember("CommentId", cvm => cvm.MapFrom(c => c.Id));
            CreateMap<CreateCommentViewModel, Comment>();
            CreateMap<UpdateCommentRequestViewModel, UpdateCommentRequest>();
            CreateMap<UpdateCommentRequest, UpdateCommentQuery>();

            // role
            CreateMap<Role, RoleViewModel>().ForMember("RoleId", rvm => rvm.MapFrom(r => r.Id));
        }
    }
}