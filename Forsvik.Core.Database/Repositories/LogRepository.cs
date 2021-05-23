using Forsvik.Core.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Database.Repositories
{
    public class LogRepository
    {
        private ArchivingContext Database;

        public LogRepository(ArchivingContext database)
        {
            Database = database;
        }

        public void AddLog(string msg, string level)
        {
            Database.Logs.Add(new Log
            {
                Id = Guid.NewGuid(),
                Level = level,
                Created = DateTime.Now,
                Message = msg
            });

            Database.SaveChanges();
        }

        public List<Log> GetLastLogs()
        {
            return Database
                .Logs
                .OrderByDescending(x => x.Created)
                .Take(1000)
                .ToList();
        }
    }
}
