using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessObjects.Models.E_com.Trading
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        //public string? AuthorName { get; set; }
        public string? ImageDir { get; set; }
        public string? VideoDir { get; set; }
        //public string? Title { get; set; }
        public string? Content { get; set; }
        //public bool PostStyle { get; set; }
        public bool IsTradePost { get; set; }
        public bool IsLock { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Hearts { get; set; }
        

        [ForeignKey("UserId"), JsonIgnore]
        public virtual AppUser? AppUser { get; set; }
    }
}


