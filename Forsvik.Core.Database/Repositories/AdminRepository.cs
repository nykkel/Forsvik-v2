﻿using Forsvik.Core.Model.Context;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Forsvik.Core.Utilities.Extensions;

namespace Forsvik.Core.Database.Repositories
{
    public class AdminRepository
    {
        private ArchivingContext Database;
        private ILogService LogService;

        public AdminRepository(ILogService logService, ArchivingContext database)
        {
            Database = database;
            LogService = logService;
        }

        public User AddUser(string name, string userName, bool isAdmin, string passwordHash)
        {
            if (GetUserByUserName(userName) != null)
            {
                return null;
            }
            if (!Database.Users.Any())
            {
                isAdmin = true;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                UserName = userName,
                PasswordHash = passwordHash,
                IsAdmin = isAdmin
            };

            Database.Users.Add(user);
            Database.SaveChanges();

            return user;
        }

        public User GetUserByUserName(string userName)
        {
            return Database
                .Users
                .FirstOrDefault(x => x.UserName == userName);
        }

        public List<User> GetUsers()
        {
            return Database
                .Users
                .OrderBy(z => z.Name)
                .ToList();
        }

        public User GetUser(Guid userId)
        {
            return Database.Users.Find(userId);
        }

        public void SaveUser(User user)
        {
            var entity = Database.Users.Find(user.Id);
            if (entity != null)
            {
                entity.SemanticCopy(user.Id);
            }
            else
                Database.Users.Add(user);
            
            Database.SaveChanges();
        }

        public void ResetPassword(Guid userId, string password)
        {
            var user = GetUser(userId);
            user.PasswordHash = GeneratePasswordHash(password);
            
            SaveUser(user);
        }

        public void SetUserAdmin(string userName, bool isAdmin)
        {
            var user = GetUserByUserName(userName);
            user.IsAdmin = isAdmin;
            SaveUser(user);
        }

        private string GeneratePasswordHash(string password)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                var bytehash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytehash)
                    sb.Append(b.ToString("X2"));

                return sb.ToString();
            }
        }

        public void DeleteUser(Guid userId)
        {
            Database.Users.Remove(Database.Users.First(u => u.Id == userId));
            Database.SaveChanges();
        }
    }
}
