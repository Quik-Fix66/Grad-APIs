using System;
using BusinessObjects.Enums;
using BusinessObjects.Models.Trading;

namespace APIs.Services.Interfaces
{
	public interface ITradeService
	{
        Task<int> AddNewTradeDetailsAsync(TradeDetails data);
        Task<int> SetTradeStatus(TradeStatus status, Guid recordId);
    }
}

