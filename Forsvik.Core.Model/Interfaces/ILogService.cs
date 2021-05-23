using Forsvik.Core.Model.External;
using System.Collections.Generic;

namespace Forsvik.Core.Model.Interfaces
{
    public interface ILogService
    {
        void Error(string message);
        List<LogModel> GetLogs();
        void Info(string message);
        void Warning(string message);
    }
}