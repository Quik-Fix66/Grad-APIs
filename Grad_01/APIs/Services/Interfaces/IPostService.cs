using APIs.Utils.Paging;
using BusinessObjects.DTO;
using BusinessObjects.DTO.Trading;
using BusinessObjects.Models;
using BusinessObjects.Models.Creative;
using BusinessObjects.Models.E_com.Trading;
using BusinessObjects.Models.Trading;
using DataAccess.DAO.E_com;

namespace APIs.Services.Interfaces
{
    public interface IPostService
    {
        //---------------------------------------------POST-------------------------------------------------------//

        Task<PagedList<Post>> GetAllPostAsync(PagingParams param);

        Task<Post?> GetPostByIdAsync(Guid postId);

        Task<int> AddNewPostAsync(Post post);

        Task<int> UpdatePostAsync(Post post);

        Task<int> DeletePostByIdAsync(Guid postId);

        Task<string> GetOldImgPathAsync(Guid postId);

        Task<string> GetOldVideoPathAsync(Guid postId);

        Task<bool> IsTradePostAsync(Guid postId);

        ////---------------------------------------------COMMENT-------------------------------------------------------//

        Task<PagedList<CommentDetailsDTO>> GetCommentByPostIdAsync(Guid postId, PagingParams @params);

        Task<int> AddCommentAsync(Comment comment);

        //public int UpdateComment(Comment comment);

        Task<int> DeleteCommentByIdAsync(Guid commentId);
        ////---------------------------------------------POSTCOMMENT-------------------------------------------------------//
        Task<int> AddNewCommentRecord(Guid cmtId, Guid postId);
        Task<int> DeleteCommentRecord(Guid cmtId);


        ////---------------------------------------------POSTINTEREST-------------------------------------------------------//

        Task<int> AddNewInteresterAsync(PostInterester postInterest);

        int UpdateInterester(PostInterester postInterest);

        Task<int> DeleteInteresterByIdAsync(Guid postInterestId);

        Task<PagedList<PostInterester>> GetInteresterByPostIdAsync(Guid postId, PagingParams @params);
    }
}