using System;
using BusinessObjects;
using BusinessObjects.Enums;
using BusinessObjects.Models.Trading;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO.Trading
{
	public class TradeDetailsDAO
	{
		private readonly AppDbContext _context;
		public TradeDetailsDAO()
		{
			_context = new AppDbContext();
		}

		public async Task<int> AddNewTradeDetailsAsync(TradeDetails data)
		{
			await _context.TradeDetails.AddAsync(data);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> SetTradeStatus(TradeStatus status, Guid recordId)
		{
			TradeDetails? record = await _context.TradeDetails.SingleOrDefaultAsync(r => r.TradeDetailId == recordId);
			if(record != null)
			{
				record.Status = status;
			}
			return await _context.SaveChangesAsync();
		}
	}
}

