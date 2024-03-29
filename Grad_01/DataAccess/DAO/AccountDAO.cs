﻿using System;
using BusinessObjects;
using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
	public class AccountDAO
	{
        //Get all user
		public List<AppUser> GetAllUsers()
		{
			List<AppUser> listUser = new List<AppUser>();
            try
            {
                using (var context = new AppDbContext())
                {
                    listUser = context.AppUsers.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listUser;
        }

        //Create a new User
        public void CreateAccount(AppUser user)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    if(context.AppUsers.ToList().Count() == 0)
                    {
                        context.AppUsers.Add(user);
                        context.SaveChanges();
                    }
                    if (user.Email != null && !CheckEmail(user.Email) && !CheckUsername(user.Username))
                    {
                        context.AppUsers.Add(user);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Check if username existed, if existed return true else return false
        public bool CheckUsername(string username)
        {
            bool result = false;
            try
            {
                using(var context = new AppDbContext())
                {
                    context.AppUsers.Any(a => a.Username == username);
                }
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        //Check if email existed, if existed return true else return false
        public bool CheckEmail(string email)
        {
            bool result = false;
            try
            {
                using (var context = new AppDbContext())
                {
                    result = context.AppUsers.Any(a => a.Email == email);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        //Find user by email then return user object
        public AppUser? FindUserByEmailAsync(string email)
        {
            AppUser? user = null;
            try
            {
                using (var context = new AppDbContext())
                {
                    if (CheckEmail(email))
                    {
                        user = context.AppUsers.Where(u => u.Email.Equals(email)).FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return user;
        }

        public string GetAccountSalt(Guid userId)
        {
            string salt;
            try
            {
                using(var context = new AppDbContext())
                {
                    salt = context.Database.SqlQuery<string>($"select Salt from AppUser where UserId = {userId}").ToString();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return salt;
        }

        public Role GetRolesDetails(string roleName)
        {
            Role? role = new Role();
            try
            {
                using (var context = new AppDbContext())
                {
                    role = context.Roles.Where(r => r.RoleName == roleName).FirstOrDefault();
                }

            }
            catch(Exception e){
                throw new Exception(e.Message);
            }
            return role;
        }

        public void AddNewRole(Role role)
        {
            try
            {
                using(var context = new AppDbContext())
                {
                    context.Roles.Add(role);
                    context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string? GetNameById(Guid userId)
        {
            try
            {
                string? result = "";
                using(var context = new AppDbContext())
                {
                    result = context.AppUsers.Where(u => u.UserId == userId).FirstOrDefault()?.Username;
                }
                return result;
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Account validation
        public int SetIsAccountValid(bool choice, Guid userId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    AppUser? user = context.AppUsers.Where(u => u.UserId == userId).SingleOrDefault();
                    if(user != null)
                    {
                        user.IsValidated = choice;
                    }
                    return context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsUserValidated(Guid userId)
        {
            try
            {
                using(var context = new AppDbContext())
                {
                    AppUser? user = context.AppUsers.Where(u => u.UserId == userId).SingleOrDefault();
                    if(user != null)
                    {
                        return user.IsValidated;
                    }
                    return false;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Agency registration
        public int SetIsAgency(bool choice, Guid userId)
        {
            try
            {
                using(var context = new AppDbContext())
                {
                    AppUser? user = context.AppUsers.Where(u => u.UserId == userId).SingleOrDefault();
                    if(user != null)
                    {
                        user.IsSeller = choice;
                    }
                    return context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool IsSeller(Guid userId)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    AppUser? user = context.AppUsers.Where(u => u.UserId == userId).SingleOrDefault();
                    if (user != null)
                    {
                        return user.IsSeller;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

