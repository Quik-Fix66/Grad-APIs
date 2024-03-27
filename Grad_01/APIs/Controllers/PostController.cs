using System.Text.RegularExpressions;
using APIs.Services;
using APIs.Services.Interfaces;
using APIs.Utils.Paging;
using BusinessObjects.DTO;
using BusinessObjects.DTO.Trading;
using BusinessObjects.Models.E_com.Trading;
using BusinessObjects.Models.Trading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IAccountService _accountService;
        private readonly ICloudinaryService _cloudinaryService;
        public PostController(IPostService postService, ICloudinaryService cloudinaryService, IAccountService accountService)
        {
            _cloudinaryService = cloudinaryService;
            _postService = postService;
            _accountService = accountService;
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
          return Ok( await _postService.GetPostByIdAsync(postId));
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
                    Guid cmtId = Guid.NewGuid();
                    int cmtChanges = await _postService.AddCommentAsync(new Comment()
                    {
                        CommentId = cmtId,
                        CommenterId = comment.CommenterId,
                        Content = comment.Content,
                        CreateDate = DateTime.Now,
                    });
                    if (cmtChanges > 0)
                    {
                    int recordChanges = await _postService.AddNewCommentRecord(cmtId, comment.PostId);
                    if (recordChanges > 0) return Ok("Successful!");
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

        //    [HttpGet("get-post-interest-by-post-id")]
        //    public IActionResult GetPostInterestByPostId(Guid postId, [FromQuery] PagingParams @params)
        //    {
        //        try
        //        {
        //            var postInterest = _postService.GetPostInterestByPostId(postId, @params);


        //            if (postInterest != null)
        //            {
        //                var metadata = new
        //                {
        //                    postInterest.TotalCount,
        //                    postInterest.PageSize,
        //                    postInterest.CurrentPage,
        //                    postInterest.TotalPages,
        //                    postInterest.HasNext,
        //                    postInterest.HasPrevious
        //                };
        //                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        //                return Ok(postInterest);
        //            }
        //            else return BadRequest("No chapter!!!");
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    [HttpPost("add-post-interest")]
        //    public IActionResult AddNewPostInterest([FromForm] AddPostInterestDTO postInterest)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                int result = _postService.AddNewPostInterest(new PostInterest()
        //                {
        //                    PostInterestId = Guid.NewGuid(),
        //                    PostId = postInterest.PostId,
        //                    InteresterId = postInterest.InteresterId,
        //                });
        //                if (result > 0)
        //                {
        //                    return Ok();
        //                }
        //                return BadRequest("Add false");
        //            }
        //            return BadRequest("Comment Invalid");
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception(e.Message);
        //        }
        //    }

        //    [HttpPut("update-post-interest")]
        //    public IActionResult UpdatePostInterest([FromForm] UpdatePostInterestDTO postInterest)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                PostInterest updateData = new PostInterest
        //                {
        //                    PostInterestId = postInterest.PostInterestId,
        //                    PostId = postInterest.PostId,
        //                };
        //                if (_postService.UpdatePostInterest(updateData) > 0)
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

        //    [HttpDelete("delete-post-interest")]
        //    public IActionResult DeletePostInterestById(Guid postInterestId)
        //    {
        //        try
        //        {
        //            _postService.DeletePostInterestById(postInterestId);
        //            var response = new
        //            {
        //                StatusCode = 204,
        //                Message = "Delete postInterest query was successful",
        //            };
        //            return Ok(response);
        //        }
        //        catch (Exception ex)
        //        {
        //            var response = new
        //            {
        //                StatusCode = 500,
        //                Message = "Delete postInterest query Internal Server Error",
        //                Error = ex,
        //            };
        //            return StatusCode(500, response);
        //        }
        //    }
    }
}