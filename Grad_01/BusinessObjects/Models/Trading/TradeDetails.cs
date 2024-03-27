using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;
using BusinessObjects.Enums;

namespace BusinessObjects.Models.Trading
{


    public class TradeDetails
    {
        [Key]
        public Guid TradeDetailId { get; set; }
        public Guid LockedRecordId { get; set; }
        public Guid AddressId { get; set; } 
        public string? Phone { get; set; }
        public string? Note { get; set; }
        public bool IsPostOwner { get; set; }

        [Column(TypeName = "varchar(20)")]
        public TradeStatus Status { get; set; } //Values: submited, on delivery, successful, cancel

        [ForeignKey("AddressId"), JsonIgnore]
        public Address Address { get; set; } = null!;
        [ForeignKey("LockedRecordId"), JsonIgnore]
        public PostInterester LockedRecord { get; set; } = null!;
    }
}