using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using Forsvik.Core.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forsvik.Core.Utilities.Extensions;
using Forsvik.Core.Database;
using Microsoft.Extensions.Configuration;

namespace forsvikapi.Services
{
    public class LogService : ILogService
    {
        private const string INFO = "INFO";
        private const string WARN = "Warning";
        private const string ERR = "Error";
        private readonly LogRepository _repository;

        public LogService(LogRepository repository)
        {
            _repository = repository;
        }

        public void Info(string message)
        {
            _repository.AddLog(message, INFO);
        }
        public void Warning(string message)
        {
            _repository.AddLog(message, WARN);
        }
        public void Error(string message)
        {
            _repository.AddLog(message, ERR);
        }

        public static void StaticError(string message)
        {
            var repo = new LogRepository(new ArchivingContext(Startup.Configuration.GetConnectionString("ForsvikDb")));
            repo.AddLog(message, ERR);
        }

        public List<LogModel> GetLogs()
        {
            return _repository
                .GetLastLogs()
                .Select(log => new LogModel
                {
                    Created = log.Created,
                    Level = log.Level,
                    Message = log.Message.Truncate(200)
                })
                .OrderByDescending(x => x.Created)
                .ToList();
        }
    }
}
