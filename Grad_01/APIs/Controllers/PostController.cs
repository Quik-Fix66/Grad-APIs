using System.Text.RegularExpressions;
using APIs.Services;
using APIs.Services.Interfaces;
using APIs.Utils.Paging;
using BusinessObjects.DTO;
using BusinessObjects.DTO.Trading;
using BusinessObjects.Models;
using BusinessObjects.Models.E_com.Trading;
using BusinessObjects.Models.Trading;
using Microsoft.AspNetCore.Mvc;
using BusinessObjects.Enums;
using Newtonsoft.Json;
using static BusinessObjects.DTO.Trading.TradeDTOs;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IAccountService _accountService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAddressService _addressService;
        private readonly ITradeService _tradeService;

        public PostController(IPostService postService, ICloudinaryService cloudinaryService, IAccountService accountService, IAddressService addressService, ITradeService tradeService)
        {
            _cloudinaryService = cloudinaryService;
            _addressService = addressService;
            _postService = postService;
            _accountService = accountService;
            _tradeService = tradeService;
        }
        //---------------------------------------------POST-------------------------------------------------------//

        [HttpGet("get-all-post")]
        public async Task<IActionResult> GetAllPostAsync([FromQuery] PagingParams @params)
        {
                var posts = await _postService.GetAllPostAsync(@params);

                List<PostDetailsDTO> result = new List<PostDetailsDTO>();
                foreach(var p in posts)
                {
                   var user = await _accountService.FindUserByIdAsync(p.UserId);
                   if(user != null)
                    {
                    result.Add(new PostDetailsDTO
                    {
                        PostData = p,
                        Username = user.Username,
                        AvatarDir = user.AvatarDir
                    });
                    }
                }

                if (posts != null)
                {
                    var metadata = new
                    {
                        posts.TotalCount,
                        posts.PageSize,
                        posts.CurrentPage,
                        posts.TotalPages,
                        posts.HasNext,
                        posts.HasPrevious
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    return Ok(result);
                }
                else return BadRequest("No chapter!!!");
        }

        // get post by id
        [HttpGet("get-post-by-id")]
        public async Task<IActionResult> GetPostByIdAsync(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            PostDetailsDTO result = new PostDetailsDTO();
            if (post != null) { 
                var user = await _accountService.FindUserByIdAsync(post.UserId);
                if (user != null)
                {
                    result.PostData = post;
                    result.Username = user.Username;
                    result.AvatarDir = user.AvatarDir;
                }
             }
            return Ok(result);
        }

        [HttpPost("add-new-post")]
        public async Task<IActionResult> AddNewPostAsync([FromForm] AddPostDTOs dto)
        {
            if (ModelState.IsValid)
            {
                Guid postId = Guid.NewGuid();
                string? userPost = await _accountService.GetUsernameById(dto.UserId);
            string imageDir = "";
            string videoDir = "";
            if (dto.ProductImages != null)
            {
                var saveProductResult = _cloudinaryService.UploadImage(dto.ProductImages, "Posts/" + userPost + "/" + postId + "/Images");
                if (saveProductResult.StatusCode != 200 || saveProductResult.Data == null)
                {
                    return BadRequest(saveProductResult.Message);
                }
                imageDir = saveProductResult.Data;
            }
            //add video
            if (dto.ProductVideos != null)
            {
                var saveProductResult = _cloudinaryService.UploadVideo(dto.ProductVideos, "Posts/" + userPost + "/" + postId + "/Videos");
                if (saveProductResult.StatusCode != 200 || saveProductResult.Data == null)
                {
                    return BadRequest(saveProductResult.Message);
                }
                videoDir = saveProductResult.Data;
            }
                    int result = await _postService.AddNewPostAsync(new Post()
                    {
                        UserId = dto.UserId,
                        PostId = postId,
                        Content = dto.Content,
                        IsTradePost = dto.IsTradePost,
                        Hearts = 0,
                        ImageDir = imageDir,
                        VideoDir = videoDir,
                        CreatedAt = DateTime.Now
                    });
                    if (result > 0)
                    {
                        return Ok();
                    }
                    return BadRequest("Add false");
                }
                return BadRequest("Model invalid!");
        }
        [HttpPut("update-post")]
        public async Task<IActionResult> UpdatePostAsync([FromForm] UpdatePostDTOs dto)
        {
            try
            {
                string? userPost = await _accountService.GetUsernameById(dto.UserId);
                if (ModelState.IsValid)
                {
                    Post updateData = new Post
                    {
                        PostId = dto.PostId,
                        UserId = dto.UserId,
                        Content = dto.Content,
                        IsTradePost = dto.IsTradePost,
                        CreatedAt = DateTime.Now,
                    };
                    if (dto.ProductImages != null)
                    {
                        string? oldImgPath = await _postService.GetOldImgPathAsync(dto.PostId);

                        if (oldImgPath != null && oldImgPath != "")
                        {
                            _cloudinaryService.DeleteImage(oldImgPath, "Post");
                        }
                        var cloudResponse = _cloudinaryService.UploadImage(dto.ProductImages, "Posts/" + userPost + "/" + dto.PostId + "/Images");
                        if (cloudResponse.StatusCode != 200 || cloudResponse.Data == null)
                        {
                            return BadRequest(cloudResponse.Message);
                        }
                        updateData.ImageDir = cloudResponse.Data;
                    }
                    //if (dto.ProductVideos != null)
                    //{
                    //    string? oldVidPath = await _postService.GetOldVideoPathAsync(dto.PostId);

                    //    if (oldVidPath != null && oldVidPath != "")
                    //    {
                    //        _cloudinaryService.DeleteVideo(oldVidPath, "Post");
                    //    }
                    //    var cloudResponse = _cloudinaryService.UploadVideo(dto.ProductVideos, "Posts/" + userPost + "/" + dto.PostId + "/Videos");
                    //    if (cloudResponse.StatusCode != 200 || cloudResponse.Data == null)
                    //    {
                    //        return BadRequest(cloudResponse.Message);
                    //    }
                    //    updateData.VideoDir = cloudResponse.Data;
                    //}

                    if ((await _postService.UpdatePostAsync(updateData)) > 0)
                    {
                        return Ok("Successful");
                    }
                    return BadRequest("Update fail");
                }
                return BadRequest("Model state invalid");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeletePostByIdAsync(Guid postId)
        {
            try
            {
                string oldVid = await _postService.GetOldVideoPathAsync(postId);
                string oldImg = await _postService.GetOldImgPathAsync(postId);
                CloudinaryResponseDTO res = new CloudinaryResponseDTO();

                if (oldVid != "" && oldVid != null)
                {
                   res = _cloudinaryService.DeleteVideo(oldVid, "Post");
                }

                if (oldImg != "" && oldImg != null)
                {
                    _cloudinaryService.DeleteImage(oldImg, "Post");
                }

                int changes = await _postService.DeletePostByIdAsync(postId);
                IActionResult result = (changes > 0) ? Ok("Successful! " + res.StatusCode  + res.Message + res.Data) : BadRequest("Delete fail!");
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        //---------------------------------------------COMMENT-------------------------------------------------------//

        [HttpGet("get-comment-by-post-id")]
        public async Task<IActionResult> GetCommentByPostIdAsync(Guid postId, [FromQuery] PagingParams @params)
        {
            try
            {
                var comment = await _postService.GetCommentByPostIdAsync(postId, @params);

                if (comment != null)
                {
                    var metadata = new
                    {
                        comment.TotalCount,
                        comment.PageSize,
                        comment.CurrentPage,
                        comment.TotalPages,
                        comment.HasNext,
                        comment.HasPrevious
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    return Ok(comment);
                }
                else return BadRequest("No commen's found!!!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddCommenAsync([FromBody] AddCommentDTO comment)
        {
                if (ModelState.IsValid)
                {
                    CommentDetailsDTO apiRespone = new CommentDetailsDTO();
                    Guid cmtId = Guid.NewGuid();
                    DateTime createDate = DateTime.Now;
                    var commenter = await _accountService.FindUserByIdAsync(comment.CommenterId);
                    int cmtChanges = await _postService.AddCommentAsync(new Comment()
                    {
                        CommentId = cmtId,
                        CommenterId = comment.CommenterId,
                        Content = comment.Content,
                        CreateDate = createDate,
                    });
                    if (cmtChanges > 0)
                    {
                    int recordChanges = await _postService.AddNewCommentRecord(cmtId, comment.PostId);
                    if (recordChanges > 0)
                    {
                        apiRespone.CommentId = cmtId;
                        apiRespone.CommenterId = comment.CommenterId;
                        apiRespone.Content = comment.Content;
                        apiRespone.CreateDate = createDate;
                        apiRespone.PostId = comment.PostId;

                        if(commenter != null)
                        {
                            apiRespone.Username = commenter.Username;
                            apiRespone.AvatarDir = commenter.AvatarDir;
                        }
                        return Ok(apiRespone);
                    }
                    else return Ok("Fail to add comment record, Deleted comments: " + await _postService.DeleteCommentByIdAsync(cmtId));
                    }
                    return BadRequest("Add fail!");
                }
                return BadRequest("Model invalid!");
        }

        //    [HttpPut("Update-Comment")]
        //    public IActionResult UpdateComment([FromForm] UpdateCommentDTO comment)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                Comment updateData = new Comment
        //                {
        //                    CommentId = comment.CommentId,
        //                    PostId = comment.PostId,
        //                    CommenterId = comment.CommenterId,
        //                    Description = comment.Description,
        //                    Created = DateTime.Now
        //                };
        //                if (_postService.UpdateComment(updateData) > 0)
        //                {
        //                    return Ok("Successful");
        //                }
        //                return BadRequest("Update fail");
        //            }
        //            return BadRequest("Model state invalid");
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception(e.Message);
        //        }
        //    }

        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteCommentById(Guid commentId)
        {
            try
            {
                int cmtChanges = await _postService.DeleteCommentByIdAsync(commentId);
                var response = new
                    {
                        StatusCode = 204,
                        Message = "Delete comment query was successful",
                    };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    StatusCode = 500,
                    Message = "Delete comment query Internal Server Error",
                    Error = ex,
                };
                return StatusCode(500, response);
            }
        }

        //    //---------------------------------------------POSTINTEREST-------------------------------------------------------//

        [HttpGet("get-post-interest-by-post-id")]
        public async Task<IActionResult> GetPostInterestByPostIdAsync(Guid postId, [FromQuery] PagingParams @params)
        {
            try
            {
                var postInteresters = await _postService.GetInteresterByPostIdAsync(postId, @params);
                List<InteresterDetailsDTO> result = new List<InteresterDetailsDTO>();

                foreach (var p in postInteresters)
                {
                    var user = await _accountService.FindUserByIdAsync(p.InteresterId);
                    if(user != null)
                    {
                        result.Add(new InteresterDetailsDTO
                        {
                            RecordId = p.PostInterestId,
                            UserId = p.InteresterId,
                            AvatarDir = user.AvatarDir,
                            Username = user.Username,
                            CreateDate = p.CreateDate
                        });
                    }
                }

                if (postInteresters != null)
                {
                    var metadata = new
                    {
                        postInteresters.TotalCount,
                        postInteresters.PageSize,
                        postInteresters.CurrentPage,
                        postInteresters.TotalPages,
                        postInteresters.HasNext,
                        postInteresters.HasPrevious
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                    return Ok(result);
                }
                else return BadRequest("No Interester!!!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("add-post-interester")]
        public async Task<IActionResult> AddNewPostInterestAsync([FromForm] AddPostInterestDTO postInterest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(await _postService.IsTradePostAsync(postInterest.PostId))
                    {
                        if(!await _postService.IsLockedPostAsync(postInterest.PostId))
                        {
                            int result = await _postService.AddNewInteresterAsync(new PostInterester()
                            {
                                PostInterestId = Guid.NewGuid(),
                                PostId = postInterest.PostId,
                                InteresterId = postInterest.InteresterId,
                                CreateDate = DateTime.Now
                            });
                            if (result > 0)
                            {
                                return Ok("Successful!");
                            }
                            return BadRequest("Add false");
                        }
                        return BadRequest("Post is locked!");
                    }
                    return BadRequest("Not a trade post!");
                }
                return BadRequest("Comment Invalid");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("accept-trade")]
        public async Task<IActionResult> AccepTradeAsync([FromBody] AcceptTradeDTO dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int lockResult = await _postService.SetLockPostAsync(true, dto.PostId);
                    if(lockResult > 0)
                    {
                        Guid? recordId = await _postService.GetLockedRecordIdAsync(dto.InteresterId, dto.PostId);
                        if(recordId != null)
                        {
                            int isChosenChanges = await _postService.SetIsChosen(true, (Guid)recordId);
                            if (isChosenChanges > 0)
                            {
                                return Ok(dto.PostId);
                            }
                            else {

                                return BadRequest("Fail to set chosen trader! revert lock post: " + await _postService.SetLockPostAsync(false, dto.PostId));
                            }
                        } return BadRequest("Lock record not found! revert lock post: " + await _postService.SetLockPostAsync(false, dto.PostId));
                        
                    } return BadRequest("Fail to lock post!");
                } return BadRequest("Model invalid!");
            }
            catch(Exception e){
                throw new Exception(e.Message);
            }
        }

        [HttpPost("submit-trade-details")]
        public async Task<IActionResult> SubmitTradeDetails([FromBody] SubmitTradeDetailDTO dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Guid recordId = Guid.NewGuid();
                    Guid addressId = Guid.NewGuid();
                    bool isPostOwner = await _postService.IsPostOwnerAsync(dto.PostId, dto.TraderId);

                    int addressChanges = _addressService.AddNewAddress(new Address
                    {
                        AddressId = addressId,
                        City_Province = dto.City_Province,
                        District = dto.District,
                        SubDistrict = dto.SubDistrict,
                        Rendezvous = dto.Rendezvous,
                        UserId = dto.TraderId
                    });
                    if(addressChanges > 0)
                    {
                        Guid? lockedRecordId = await _postService.GetLockedRecordIdAsync(dto.TraderId, dto.PostId);
                        if(lockedRecordId != null)
                        {
                            TradeDetails details = new TradeDetails
                            {
                                TradeDetailId = recordId,
                                AddressId = addressId,
                                IsPostOwner = isPostOwner,
                                Status = TradeStatus.Submited,
                                LockedRecordId = (Guid)lockedRecordId,
                                Note = dto.Note,
                                Phone = dto.Phone
                            };
                            int tradeDetailsChanges = await _tradeService.AddNewTradeDetailsAsync(details);
                            if (tradeDetailsChanges > 0) return Ok(details);
                            else
                            {
                                
                                return BadRequest("Fail to add trade details! Address deleted: " + await _addressService.DeleteAddressAsync(addressId));
                            }
                            
                        }return BadRequest("Locked record not found! Address deleted:" + await _addressService.DeleteAddressAsync(addressId));
                    } return BadRequest("Fail to add address!");

                } return BadRequest("Model state invalid!");
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        [HttpDelete("delete-post-interest")]
        public IActionResult DeletePostInterestByIdAsync(Guid postInterestId)
        {
            try
            {
                _postService.DeleteInteresterByIdAsync(postInterestId);
                var response = new
                {
                    StatusCode = 204,
                    Message = "Delete postInterest query was successful",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    StatusCode = 500,
                    Message = "Delete postInterest query Internal Server Error",
                    Error = ex,
                };
                return StatusCode(500, response);
            }
        }
    }
}