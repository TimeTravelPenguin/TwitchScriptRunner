#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ConsoleService.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-23 9:18 AM

#endregion

#region usings

using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Logging;

#endregion

namespace TwitchScriptRunner
{
  internal class ConsoleService : ConsoleAppBase
  {
    private readonly ILogger<ConsoleService> _logger;

    public ConsoleService(ILogger<ConsoleService> logger)
    {
      _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken = new())
    {
      Console.Title = "AHK Script Runner for Twitch";
      _logger.LogInformation("Application service is running");

      Console.WriteLine(Environment.NewLine + "Press ESC to terminate the application" + Environment.NewLine);

      while (true)
      {
        var kp = Console.ReadKey(true);

        if (kp.Key == ConsoleKey.Escape)
        {
          break;
        }
      }

      return Task.CompletedTask;
    }
  }
}