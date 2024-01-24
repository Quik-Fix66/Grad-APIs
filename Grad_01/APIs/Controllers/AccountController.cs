﻿using System;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using APIs.DTO;
using APIs.Services.Intefaces;
using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
	{
		private readonly IAccountService accService;

        public AccountController(IAccountService service)
        {
            accService = service;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDTO model)
        {
            var status = new Status();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            // check if users exists
            var userExists = accService.FindUserByEmailAsync(model.Email);
            if (userExists.Username != null)
            {
                status.StatusCode = 0;
                status.Message = userExists.Username;
                return Ok(status);
            }
            AppUser user = accService.Register(model);
            return Ok(user);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO model)
        {
            var status = new Status();
            AppUser user = new AppUser();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            user = accService.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            byte[] salt = Convert.FromHexString(user.Salt);
            if (!accService.VerifyPassword(model.Password, user.Password, salt, out byte[] result))
            {
                return BadRequest("Wrong password" + Convert.ToHexString(result));
            }
            string token = accService.CreateToken(user);
            var refreshToken = accService.GenerateRefreshToken();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiredDate
            };
            Response.Cookies.Append("refreshToken", refreshToken.RefreshToken, cookieOptions);

            //Add refreshtoken to database from Service -> dao
            return Ok(token);

        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddNewRole([FromBody] RoleDTO data)
        {
            try
            {
                var status = new Status();
                if (!ModelState.IsValid)
                {
                    status.StatusCode = 0;
                    status.Message = "Please pass all the required fields";
                    return Ok(status);
                }
                if (accService.GetRoleDetails(data.RoleName) is not null)
                {
                    return BadRequest("Role already existed");
                }
                Role role = new Role()
                {
                    RoleId = Guid.NewGuid(),
                    RoleName = data.RoleName,
                    Description = data.Description
                };

                accService.AddNewRole(role);
                return Ok("New role added");
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet]
        [Route("get-default-address")]
        public IActionResult GetDefaultAddress(Guid userId)
        {
            try
            {
                Address? address = accService.GetDefaultAddress(userId);
                if (address != null)
                {
                    return Ok(address);
                }
                else return BadRequest("Default Address' not set!!!");
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("upload-cic")]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files, Guid userId)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

        [HttpGet, Authorize]
        [Route("get-user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
                var roleClaim = HttpContext.User.FindFirst(ClaimTypes.Role);
                var usernameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
                var emailClaim = HttpContext.User.FindFirst(ClaimTypes.Email);
                if (userIdClaim != null)
                {
                    var userId = Guid.Parse(userIdClaim.Value);
                    if (roleClaim != null)
                    {
                        if (usernameClaim != null)
                        {
                          Address address = accService.GetDefaultAddress(userId);
                            UserProfile profile = new UserProfile()
                            {
                                UserId = userId,
                                Username = usernameClaim.Value,
                                Role = roleClaim.Value,
                                Address = address.Rendezvous,
                                Email = emailClaim.Value
                            };
                            return Ok(profile);
                        }
                        else return BadRequest("Username claim not found!!!");
                    } else return BadRequest("Role claim not found!!!");
                } else return BadRequest("User ID claim not found!!!");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
