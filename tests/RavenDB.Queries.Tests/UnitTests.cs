using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RavenDB.Queries.Tests
{
    public abstract class UnitTests
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly ILogger Logger;
        protected readonly ILoggerFactory LoggerFactory;

        protected UnitTests(ITestOutputHelper outputHelper)
        {
            // Pass the ITestOutputHelper object to the TestOutput sink
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(outputHelper)
                .CreateLogger();
            
            LoggerFactory = new SerilogLoggerFactory(log);
            Logger = LoggerFactory.CreateLogger(GetType());
        }
        
        protected virtual RavenQueryBuilder Builder => new RavenQueryBuilder(LoggerFactory);

        protected void AssertJS(string expected, string actual)
        {
            Logger.LogInformation("Result: {0}", actual);
            Assert.Equal(expected, actual);
        }
    }
}