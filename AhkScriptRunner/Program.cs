#region Title Header

// Name: Phillip Smith
// 
// Solution: AhkScriptRunner
// Project: AhkScriptRunner
// File Name: Program.cs
// 
// Current Data:
// 2021-07-23 9:27 AM
// 
// Creation Date:
// 2021-07-23 8:55 AM

#endregion

#region usings

using System;
using System.IO;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

#endregion

namespace AhkScriptRunner
{
  public static class Program
  {
    public static async Task Main()
    {
      var appConfig = GetAppConfig();

      if (appConfig is null)
      {
        return;
      }

      await GenericHost
        .CreateConsoleHostBuilder()
        .ConfigureServices(services =>
        {
          services.AddSingleton<IConsoleApp, ConsoleService>();
          services.AddHostedService<ConsoleClient>();
          services.AddSingleton(p => appConfig);
        })
        .RunConsoleAsync(options => options.SuppressStatusMessages = true);

      Console.WriteLine("Application has finished executing. Press any key to exit...");
      Console.ReadKey(true);
      Environment.Exit(0);
    }

    private static AppConfig GetAppConfig()
    {
      var configFile = Path.Combine(AppContext.BaseDirectory, "appconfig.json");

      if (!File.Exists(configFile))
      {
        Console.WriteLine("Creating 'appconfig.json'. Please modify the fields within and relaunch the application.");

        var serialized =
          JsonConvert.SerializeObject(
            new AppConfig
            {
              ApplicationClientId = "dev.twitch.tv/console/apps client id here",
              ChannelName = "your channel name"
            },
            Formatting.Indented);

        File.WriteAllText(configFile, serialized);

        Environment.Exit(0);
        return null;
      }

      var configData = File.ReadAllText(configFile);
      AppConfig botConfiguration;
      try
      {
        botConfiguration = JsonConvert.DeserializeObject<AppConfig>(configData);
      }
      catch (Exception e)
      {
        Console.WriteLine("There is an error with the formatting of 'appconfig.json'." + Environment.NewLine);
        Console.WriteLine(e.Message);
        Environment.Exit(0);
        return null;
      }

      return botConfiguration;
    }
  }
}