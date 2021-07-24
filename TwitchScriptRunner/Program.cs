#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: Program.cs
// 
// Current Data:
// 2021-07-24 5:04 PM
// 
// Creation Date:
// 2021-07-23 8:55 AM

#endregion

#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;
using TwitchScriptRunner.Helpers;

#endregion

namespace TwitchScriptRunner
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

      var host = GenericHost
        .CreateConsoleHostBuilder()
        .ConfigureServices(services =>
        {
          services.AddSingleton<IConsoleApp, ConsoleService>();
          services.AddHostedService<ConsoleClient>();
          services.AddSingleton(p => appConfig);
          services.AddScoped<ITwitchAPI, TwitchAPI>();
          services.AddScoped<ITwitchPubSub, TwitchPubSub>();
        });

      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("This application was made by TimeTravelPenguin.");
      Console.WriteLine("Checkout the application repository: https://github.com/TimeTravelPenguin/AhkScriptRunner");
      Console.WriteLine(
        "Feel free to leave a tip or donation -- it helps me buy textbooks for university! Of course, you don't have to!");
      Console.WriteLine("Donation page: https://ko-fi.com/timetravelpenguin");
      Console.WriteLine(Environment.NewLine);
      Console.ForegroundColor = ConsoleColor.White;

      await host.RunConsoleAsync(options => options.SuppressStatusMessages = true);

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

        var newConfig = new AppConfig
        {
          ApplicationClientId = "dev.twitch.tv/console/apps client id here",
          ChannelName = "your channel name",
          ApplicationClientSecret = "dev.twitch.tv/console/apps client secret here",
          OAuthAccessToken = "get an access token from here https://twitchtokengenerator.com",
          ScriptDirectory = new ScriptDir {ScriptDirParentPath = Path.Combine(AppContext.BaseDirectory, "Scripts")}
        };

        var serialized = JsonConvert.SerializeObject(newConfig, Formatting.Indented);

        File.WriteAllText(configFile, serialized);

        ValidateScriptDirs(newConfig.ScriptDirectory.ScriptDirParentPath);

        Environment.Exit(0);
        return null;
      }

      var configData = File.ReadAllText(configFile);
      AppConfig appConfig;
      try
      {
        appConfig = JsonConvert.DeserializeObject<AppConfig>(configData);
      }
      catch (Exception e)
      {
        Console.WriteLine("There is an error with the formatting of 'appconfig.json'." + Environment.NewLine);
        Console.WriteLine(e.Message);
        Environment.Exit(0);
        return null;
      }

      if (appConfig is null)
      {
        throw new InvalidOperationException($"Variable '{nameof(appConfig)}' is null");
      }

      if (!ValidateScriptDirs(appConfig.ScriptDirectory.ScriptDirParentPath))
      {
        throw new InvalidOperationException("Error validating script subdirectories");
      }

      return appConfig;
    }

    private static bool ValidateScriptDirs(string scriptDir)
    {
      var scriptSubDirs = ApiListeningEventHelper.GetAllEventScriptPaths(scriptDir);

      if (!Directory.Exists(scriptDir))
      {
        var allDirs = new List<string>(scriptSubDirs);
        allDirs.Insert(0, scriptDir);

        ListMissingDirs(allDirs);

        Environment.Exit(0);
        return false;
      }

      var notExistPath = new List<string>();
      foreach (var scriptSubDir in scriptSubDirs)
      {
        if (!Directory.Exists(scriptSubDir))
        {
          Directory.CreateDirectory(scriptSubDir);
          notExistPath.Add(scriptSubDir);
        }
      }

      if (notExistPath.Any())
      {
        ListMissingDirs(notExistPath);

        Environment.Exit(0);
        return false;
      }

      return true;
    }

    private static void ListMissingDirs(IEnumerable<string> allDirs)
    {
      Console.WriteLine("The directories");
      foreach (var scriptSubDir in allDirs)
      {
        Console.WriteLine($"\t{scriptSubDir}");
        Directory.CreateDirectory(scriptSubDir);
      }

      Console.WriteLine("did not exist, but have been created.");
      Console.WriteLine("Place .ahk and .py files inside the respective subdirectories and relaunch the program.");
    }
  }
}