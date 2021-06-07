using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using Forsvik.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace forsvikapi.Services
{
    public class UserService
    {
        private readonly ILogService Log;

        private readonly AdminRepository Repository;

        public UserService(ILogService logService, AdminRepository repository)
        {
            Log = logService;
            Repository = repository;
        }


        public CurrentUserModel IsValidUser(LoginModel login)
        {
            var user = new CurrentUserModel();

            if (IsDefaultFirstTimeUser())
            {
                user.IsAdmin = true;
                user.IsFirstTimeUser = true;
                return user;
            }

            var dbUser = Repository.GetUserByUserName(login.Email);
            if (dbUser == null)
            {
                return null;
            }

            user.SemanticCopy(dbUser);

            var hash = GeneratePasswordHash(login.Password);

            if (dbUser.PasswordHash == hash)
                return user;

            return null;
        }

        internal void AddUser(UserModel user)
        {
            Repository.AddUser(user.Name, user.UserName, user.IsAdmin, GeneratePasswordHash(user.Password));
        }

        internal void SetUserAdmin(Guid id, bool isAdmin)
        {
            var user = Repository.GetUser(id);
            user.IsAdmin = isAdmin;
            Repository.SaveUser(user);

            Log.Info("Admin rights changed for user " + user.UserName);
        }

        internal List<UserListItem> GetUsers()
        {
            return Repository
                .GetUsers()
                .Select(u => new UserListItem
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName,
                    IsAdmin = u.IsAdmin
                }).ToList();
        }

        internal void ResetPassword(Guid userId, string password)
        {
            var user = Repository.GetUser(userId);
            user.PasswordHash = GeneratePasswordHash(password);
            Log.Info("Password resetted for user " + user.UserName);

            Repository.SaveUser(user);
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

        public bool IsDefaultFirstTimeUser()
        {
            return !Repository.GetUsers().Any();
        }
    }
}
