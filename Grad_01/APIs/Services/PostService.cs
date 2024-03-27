﻿using APIs.Services.Interfaces;
using APIs.Utils.Paging;
using BusinessObjects.DTO.Trading;
using BusinessObjects.Models.E_com.Trading;
using BusinessObjects.Models.Trading;
using DataAccess.DAO.E_com;
using DataAccess.DAO.Trading;

namespace APIs.Services
{
    public class PostService : IPostService
    {
        private readonly PostDAO _postDAO;
        private readonly PostCommentDAO _postCommentDAO;
        private readonly CommentDAO _commentDAO;
        public PostService()
        {
            _postCommentDAO = new PostCommentDAO();
            _commentDAO = new CommentDAO();
            _postDAO = new PostDAO();
        }

        //---------------------------------------------POST-------------------------------------------------------//

        public async Task<PagedList<Post>> GetAllPostAsync(PagingParams param)
        {
            return PagedList<Post>.ToPagedList((await _postDAO.GetAllPostAsync()).OrderBy(c => c.CreatedAt).AsQueryable(), param.PageNumber, param.PageSize);
        }

        public async Task<int> AddNewPostAsync(Post post) => await _postDAO.AddNewPostAsync(post);

        public async Task<int> DeletePostByIdAsync(Guid postId) => await _postDAO.DeletePostByIdAsync(postId);

        public async Task<Post?> GetPostByIdAsync(Guid postId) => await _postDAO.GetPostByIdAsync(postId);

        public async Task<int> UpdatePostAsync(Post post) => await _postDAO.UpdatePostAsync(post);

        public async Task<string> GetOldImgPathAsync(Guid postId) => await _postDAO.GetOldImgPathAsync(postId);

        public async Task<string> GetOldVideoPathAsync(Guid postId) => await _postDAO.GetOldVideoPathAsync(postId);

        //---------------------------------------------COMMENT-------------------------------------------------------//

        public async Task<PagedList<CommentDetailsDTO>> GetCommentByPostIdAsync(Guid postId, PagingParams @params)
        {
            return PagedList<CommentDetailsDTO>.ToPagedList((await _commentDAO.GetCommentByPostIdAsync(postId))?.OrderBy(c => c.CreateDate).AsQueryable(), @params.PageNumber, @params.PageSize);
        }

        public async Task<int> AddCommentAsync(Comment comment) => await _commentDAO.AddCommentAsync(comment);


        //public async Task<int> UpdateCommentAsync(Comment comment) => new CommentDAO().UpdateComment(comment);

        public async Task<int> DeleteCommentByIdAsync(Guid commentId) => await _commentDAO.DeleteCommentByIdAsync(commentId);

        ////---------------------------------------------POSTCOMMENT-------------------------------------------------------//
        public async Task<int> AddNewCommentRecord(Guid cmtId,Guid postId) => await _postCommentDAO.AddNewRecordAsync(cmtId, postId);
        public async Task<int> DeleteCommentRecord(Guid cmtId) => await _postCommentDAO.DeleteRecordAsync(cmtId);

        ////---------------------------------------------POSTINTEREST-------------------------------------------------------//

        //public int AddNewPostInterest(PostInterest postInterest) => new PostInterestDAO().AddNewPostInterest(postInterest);

        //public int UpdatePostInterest(PostInterest postInterest) => new PostInterestDAO().UpdatePostInterest(postInterest);

        //public int DeletePostInterestById(Guid postInterestId) => new PostInterestDAO().DeletePostInterestById(postInterestId);

        //public PagedList<PostInterest> GetPostInterestByPostId(Guid postId, PagingParams @params)
        //{
        //    return PagedList<PostInterest>.ToPagedList(new PostInterestDAO().GetPostInterestByPostId(postId)?.OrderBy(ch => ch.PostInterestId).AsQueryable(), @params.PageNumber, @params.PageSize);
        //}

    }
}
