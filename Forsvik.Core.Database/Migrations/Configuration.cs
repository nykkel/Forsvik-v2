//using System.Security.Cryptography;
//using System.Text;
//using Forsvik.Core.Model.Context;

//namespace Forsvik.Core.Database.Migrations
//{
//    using System;
//    using System.Data.Entity;
//    using System.Data.Entity.Migrations;
//    using System.Linq;

//    internal sealed class Configuration : DbMigrationsConfiguration<ArchivingContext>
//    {
//        public Configuration()
//        {
//            AutomaticMigrationsEnabled = false;
//        }

//        protected override void Seed(ArchivingContext context)
//        {
//            if (!context.Users.Any())
//            {
//                context.Users.Add(new User
//                {
//                    Id = Guid.NewGuid(),
//                    IsAdmin = true,
//                    Name = "Admin",
//                    UserName = "admin",
//                    PasswordHash = GeneratePasswordHash("fossvik")
//                });
//            }
//        }

//        private string GeneratePasswordHash(string password)
//        {
//            using (HashAlgorithm algorithm = SHA256.Create())
//            {
//                var bytehash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
//                StringBuilder sb = new StringBuilder();
//                foreach (byte b in bytehash)
//                    sb.Append(b.ToString("X2"));

//                return sb.ToString();
//            }
//        }
//    }
//}
