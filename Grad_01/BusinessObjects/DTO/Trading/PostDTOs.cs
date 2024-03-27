using BusinessObjects.Models.E_com.Trading;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.Trading
{
//-------------------------------------------------POST-----------------------------------------------------//
 
    public class AddPostDTOs
    {
        public Guid UserId { get; set; }
        public IFormFile? ProductImages { get; set; }
        public IFormFile? ProductVideos { get; set; }
        public string? Content { get; set; }
        public bool IsTradePost { get; set; }
        //public List<string> CateId { get; set; } = null!;
    }

    public class UpdatePostDTOs
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public IFormFile? ProductImages { get; set; }
        public IFormFile? ProductVideos { get; set; }
        public string? Content { get; set; }
        public bool IsTradePost { get; set; }
        //public List<string> CateId { get; set; } = null!;
    }

    public class PostDetailsDTO
    {
        public Post PostData { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? AvatarDir { get; set; }
    }
    

    //-------------------------------------------------COMMENT-----------------------------------------------------//

    public class CommentDetailsDTO
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid CommenterId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreateDate { get; set; } 
        public string Username { get; set; } = null!;
        public string? AvatarDir { get; set; }
    }

    public class AddCommentDTO
    {
        public Guid PostId { get; set; }
        public Guid CommenterId { get; set; }
        public string Content { get; set; } = null!;
    }

    //public class UpdateCommentDTO
    //{
    //    public Guid CommentId { get; set; }
    //    public Guid PostId { get; set; }
    //    public Guid CommenterId { get; set; }
    //    public string Content { get; set; }
    //}
    //-------------------------------------------------POSTINTEREST-----------------------------------------------------//
    public class InteresterDetailsDTO
    {
        public Guid RecordId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? AvatarDir { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class AddPostInterestDTO
    {
        public Guid PostId { get; set; }
        public Guid InteresterId { get; set; }
    }

    public class UpdatePostInterestDTO
    {
        public Guid PostId { get; set; }
        public Guid PostInterestId { get; set; }
        public Guid InteresterId { get; set; }
    }

    public class DeletePostInterestDTO
    {
        public Guid InteresterId { get; set; }
        public Guid PostId { get; set;}
    }

}