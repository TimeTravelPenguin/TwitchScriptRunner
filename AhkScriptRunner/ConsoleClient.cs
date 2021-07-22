#region Title Header

// Name: Phillip Smith
// 
// Solution: AhkScriptRunner
// Project: AhkScriptRunner
// File Name: ConsoleClient.cs
// 
// Current Data:
// 2021-07-23 9:26 AM
// 
// Creation Date:
// 2021-07-23 9:23 AM

#endregion

#region usings

using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace AhkScriptRunner
{
  public class ConsoleClient : ConsoleWorker
  {
    private readonly ILogger<ConsoleClient> _logger;

    public ConsoleClient(IHostApplicationLifetime applicationLifetime, ILogger<ConsoleClient> logger) 
      : base(applicationLifetime)
    {
      _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      return Task.CompletedTask;
    }
  }
}