using System;
using APIs.Services.Interfaces;
using BusinessObjects.Enums;
using BusinessObjects.Models.Trading;
using DataAccess.DAO.Trading;

namespace APIs.Services
{
	public class TradeService: ITradeService
	{
		private readonly TradeDetailsDAO _tradeDetailsDAO;
		public TradeService()
		{
			_tradeDetailsDAO = new TradeDetailsDAO();
		}

        public async Task<int> AddNewTradeDetailsAsync(TradeDetails data) => await _tradeDetailsDAO.AddNewTradeDetailsAsync(data);

		public async Task<int> SetTradeStatus(TradeStatus status, Guid recordId) => await _tradeDetailsDAO.SetTradeStatus(status, recordId);
    }
}

