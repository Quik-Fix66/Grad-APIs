using BusinessObjects;
using BusinessObjects.Models.E_com.Trading;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO.E_com
{
    public class PostDAO
    {
        private readonly AppDbContext _context;
        public PostDAO()
        {
            _context = new AppDbContext();
        }
        //Get all post
        public async Task<List<Post>> GetAllPostAsync() => await _context.Posts.ToListAsync();


        //Get post by id
        public async Task<Post?> GetPostByIdAsync(Guid postId) => await _context.Posts.SingleOrDefaultAsync(p => p.PostId == postId);

        //Add new post
        public async Task<int> AddNewPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            return await _context.SaveChangesAsync();
        }
        //Modify post
        public async Task<int> UpdatePostAsync(Post post)
        {
           
                var existingPost = await _context.Posts.FindAsync(post.PostId);
                if (existingPost != null)
                {
                    existingPost = post;
                    return await _context.SaveChangesAsync();
                } return 0;
        }

        //Delete post by id
        public async Task<int> DeletePostByIdAsync(Guid postId)
        {
            Post? post = await GetPostByIdAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<string> GetOldImgPathAsync(Guid postId)
        {
            Post? post = await _context.Posts.SingleOrDefaultAsync(c => c.PostId == postId);
            string result = (post != null && post.ImageDir != null) ?
            post.ImageDir : "";
            return result;
        }

        public async Task<string> GetOldVideoPathAsync(Guid postId)
        {
            Post? post = await _context.Posts.SingleOrDefaultAsync(c => c.PostId == postId);
            string result = (post != null && post.VideoDir != null) ?
            post.VideoDir : "";
            return result;
        }

        public async Task<bool> IsTradePostAsync(Guid postId)
        {
            Post? record = await _context.Posts.SingleOrDefaultAsync(p => p.PostId == postId);
            bool result = (record != null) ? record.IsTradePost : false;
            return result;
        }
    }
}
