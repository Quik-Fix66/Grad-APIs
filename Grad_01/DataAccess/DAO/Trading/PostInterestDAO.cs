using BusinessObjects.Models.Trading;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO.Trading
{
    public class PostInterestDAO
    {
        private readonly AppDbContext _context;
        public PostInterestDAO()
        {
            _context = new AppDbContext();
        }

        //Get postInterest by post id
        public async Task<List<PostInterester>> GetPostInterestByPostIdAsync(Guid postId)
        =>  await _context.PostInteresters.Where(p => p.PostId == postId).ToListAsync();

        //Add Interest
        public async Task<int> AddNewPostInterestAsync(PostInterester postInterest)
        { 
               await _context.PostInteresters.AddAsync(postInterest);
                    return await _context.SaveChangesAsync();
        }

        //Update PostInterest
        public int UpdatePostInterest(PostInterester postInterest)
        {
           _context.Update(postInterest);
            return _context.SaveChanges();
        }

        //Delete PostInterest by id
        public async Task<int> DeletePostInterestByIdAsync(Guid postInterestId)
        {
                    PostInterester? postInterest = await _context.PostInteresters.SingleOrDefaultAsync(c => c.PostInterestId == postInterestId);
                    if (postInterest != null)
                    {
                        _context.PostInteresters.Remove(postInterest);
                    }
                    return await _context.SaveChangesAsync();
            
        }
    }
}
