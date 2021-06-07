using Forsvik.Core.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace forsvikapi.Services
{
    public class ErrorLoggingMiddleware
    {
        public ILogService LogService { get; set; }

        public ErrorLoggingMiddleware(ILogService logService)
        {
            LogService = logService;
        }

        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                LogService.Error(e.Message);
                throw;
            }
        }
    }
}
