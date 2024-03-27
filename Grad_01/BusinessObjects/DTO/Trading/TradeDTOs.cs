using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.Trading
{
    public class TradeDTOs
    {
       public class SubmitTradeDetailDTO
        {
            public Guid PostId { get; set; }
            public Guid TraderId { get; set; }
            public string? City_Province { get; set; }
            public string? District { get; set; }
            public string? SubDistrict { get; set; }
            public string Rendezvous { get; set; } = null!;
            public string? Phone { get; set; }
            public string? Note { get; set; }
        } 
    }
}