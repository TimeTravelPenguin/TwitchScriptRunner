#region Title Header

// Name: Phillip Smith
//
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner File Name: ConsoleClient.cs
//
// Current Data: 2021-08-02 10:26 AM
//
// Creation Date: 2021-07-23 9:23 AM

#endregion Title Header

#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using AllOverIt.Process;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Interfaces;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;

#endregion usings

namespace TwitchScriptRunner
{
  internal class ConsoleClient : ConsoleWorker
  {
    private readonly AppConfig _appConfig;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<ConsoleClient> _logger;
    private readonly ITwitchAPI _twitchApi;
    private readonly ITwitchPubSub _twitchClient;
    private User _userData;

    public ConsoleClient(IHostApplicationLifetime applicationLifetime, AppConfig appConfig,
      ITwitchAPI twitchApi, ITwitchPubSub twitchClient, ILogger<ConsoleClient> logger)
      : base(applicationLifetime)
    {
      _applicationLifetime = applicationLifetime;
      _appConfig = appConfig;
      _twitchApi = twitchApi;
      _twitchClient = twitchClient;
      _logger = logger;

      _twitchApi.Settings.ClientId = _appConfig.ApplicationClientId;
      _twitchApi.Settings.Secret = _appConfig.ApplicationClientSecret;

      if (_appConfig.NextTokenRefresh.Date <= DateTime.Today.Date)
      {
        RefreshTokens();
      }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var isValid = await ConfigureListenEvents();

      if (isValid)
      {
        _logger.LogInformation("Connecting PubSub client");
        _twitchClient.Connect();
        _logger.LogInformation("Connected PubSub client");
      }
      else
      {
        _applicationLifetime.StopApplication();
      }
    }

    protected override void OnStopping()
    {
      base.OnStopping();

      _logger.LogInformation("Disconnecting PubSub client");
      _twitchClient.Disconnect();
      _logger.LogInformation("Disconnecting PubSub client");
    }

    private async Task<bool> ConfigureListenEvents()
    {
      try
      {
        _userData = (await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> { _appConfig.ChannelName }))
          .Users
          .FirstOrDefault();
      }
      catch (Exception e)
      {
        _logger.LogCritical(e.ToString());

        return false;
      }

      if (_userData is null)
      {
        _logger.LogCritical(
          $"{nameof(_appConfig.ChannelName)} with value '{_appConfig.ChannelName}' cannot be found on Twitch");

        return false;
      }

      _twitchClient.OnPubSubServiceConnected += TwitchClientOnServiceConnected;
      _twitchClient.OnListenResponse += TwitchClientOnListenResponse;
      _twitchClient.OnPubSubServiceError += TwitchClientOnServiceError;

      if (_appConfig.ApiListeningEvents.BitDonations)
      {
        _logger.LogInformation("Enabling bit event logging");
        _twitchClient.ListenToBitsEvents(_userData.Id);
        _twitchClient.OnBitsReceived += TwitchClientOnBitsReceived;
      }

      if (_appConfig.ApiListeningEvents.ChannelPointRedemption)
      {
        _logger.LogInformation("Enabling channel point redemption event logging");
        _twitchClient.ListenToRewards(_userData.Id);
        _twitchClient.OnRewardRedeemed += TwitchClientOnRewardRedeemed;
      }

      return true;
    }

    private async Task ExecuteScript(string eventName, string eventValue)
    {
      // TODO: Make dynamic for extended file types using factory and strategy patterns

      _logger.LogInformation(
        $"Executing script.{Environment.NewLine}\tEvent name: {eventName}{Environment.NewLine}\tEvent value: {eventValue}");

      var pathPy = Path.Combine(_appConfig.ScriptDirectory.ScriptDirParentPath, eventName,
        $"{eventName}{eventValue}.py");

      var pathAhk = Path.Combine(_appConfig.ScriptDirectory.ScriptDirParentPath, eventName,
        $"{eventName}{eventValue}.ahk");

      var pyExists = File.Exists(pathPy);
      var ahkExists = File.Exists(pathAhk);

      if (pyExists && ahkExists)
      {
        _logger.LogError("Two files exist with the same name but different extension:"
                         + Environment.NewLine + pathPy
                         + Environment.NewLine + pathAhk);
        return;
      }

      if (pyExists ^ ahkExists)
      {
        var process = pyExists ? "python" : "autohotkey";
        var script = pyExists ? pathPy : pathAhk;
        try
        {
          await Process.ExecuteAndWaitAsync(AppContext.BaseDirectory, process, $"\"{script}\"", 10000);
        }
        catch (Exception exception)
        {
          _logger.LogCritical($"An error occurred during the execution of '{script}'."
                              + $"{Environment.NewLine}Process: {process}"
                              + $"{Environment.NewLine}{exception}");
        }
      }
    }

    private void RefreshTokens()
    {
      _logger.LogInformation("Refreshing token...");

      var response = _twitchApi.ThirdParty.AuthorizationFlow.RefreshToken(_appConfig.OAuthRefreshToken);
      _appConfig.OAuthAccessToken = response.Token;
      _appConfig.OAuthRefreshToken = response.Refresh;
      _appConfig.NextTokenRefresh = DateTime.Today.AddDays(60);

      _logger.LogInformation($"Token refresh completed. Next refresh: {_appConfig.NextTokenRefresh.Date}");

      UpdateAppConfig();
    }

    private async void TwitchClientOnBitsReceived(object? sender, OnBitsReceivedArgs e)
    {
      _logger.LogInformation($"Bits received: {e.BitsUsed} from {e.Username}");
      await ExecuteScript(nameof(ApiListeningEvents.BitDonations), e.BitsUsed.ToString());
    }

    private void TwitchClientOnListenResponse(object? sender, OnListenResponseArgs e)
    {
      if (!e.Successful)
      {
        _logger.LogError($"{e.Response.Error}: {e.Topic}");
      }
    }

    private async void TwitchClientOnRewardRedeemed(object? sender, OnRewardRedeemedArgs e)
    {
      _logger.LogInformation($"Reward redeemed: {e.RewardTitle} by {e.DisplayName}");
      await ExecuteScript(nameof(ApiListeningEvents.ChannelPointRedemption), e.RewardTitle.Replace(" ", ""));
    }

    private void TwitchClientOnServiceConnected(object? sender, EventArgs e)
    {
      _twitchClient.SendTopics(_appConfig.OAuthAccessToken);
    }

    private void TwitchClientOnServiceError(object? sender, OnPubSubServiceErrorArgs e)
    {
      _logger.LogError(e.Exception.Message);
    }

    private void UpdateAppConfig()
    {
      File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "appconfig.json"),
        JsonConvert.SerializeObject(_appConfig, Formatting.Indented));

      _logger.LogInformation("Updated appconfig.json");
    }
  }
}