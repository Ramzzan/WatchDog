﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src
{
    public class WatchDogExceptionLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public WatchDogExceptionLogger(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<WatchDogExceptionLogger>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogException(ex);
                throw;
            }
        }
        public async Task LogException(Exception ex)
        {
            Debug.WriteLine("The following exception is logged: " + ex.Message);
            var watchExceptionLog = new WatchExceptionLog();
            watchExceptionLog.EncounteredAt = DateTime.Now;
            watchExceptionLog.Message = ex.Message;
            watchExceptionLog.StackTrace = ex.StackTrace;
            watchExceptionLog.Source = ex.Source;


            //Insert
            LiteDBHelper.InsertWatchExceptionLog(watchExceptionLog);
        }
    }
}
