using System;
using BusinessObjects;
using BusinessObjects.Models.Ecom.Rating;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO.Ecom
{
	public class RatingDAO
	{
        private readonly AppDbContext _context;
        public RatingDAO()
        {
            _context = new AppDbContext();
        }
		public double? GetOverallRatingById(Guid ratingId)
        {
            try
			{
                double? result = null;

                using (var context = new AppDbContext())
				{
                    Rating? rate = context.Ratings.Where(r => r.RatingId == ratingId).FirstOrDefault();
                    if(rate != null)
					{
                        result = rate?.OverallRating;
                    }
                }
                return result;
            }
            catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

        public async Task<int> AddNewUserRating(RatingRecord record)
        {
            await _context.RatingRecords.AddAsync(record);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateUserRating(RatingRecord record)
        {
            _context.RatingRecords.Update(record);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteUserRating(Guid recordId)
        {
            RatingRecord? record = await _context.RatingRecords.SingleOrDefaultAsync(r => r.RatingRecordId == recordId);
            if(record != null)
            {
                _context.RatingRecords.Remove(record);
            }
            return await _context.SaveChangesAsync();
        }
	}
}

