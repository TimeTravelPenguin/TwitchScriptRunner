#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ConsoleService.cs
// 
// Current Data:
// 2021-07-24 6:58 PM
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
      Console.Title = "Script Runner for Twitch";
      _logger.LogInformation("Application service is running");

      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(Environment.NewLine + "\tPress ESC to terminate the application" + Environment.NewLine);
      Console.ForegroundColor = ConsoleColor.White;

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