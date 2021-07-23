#region Title Header

// Name: Phillip Smith
// 
// Solution: AhkScriptRunner
// Project: AhkScriptRunner
// File Name: ConsoleClient.cs
// 
// Current Data:
// 2021-07-23 11:20 AM
// 
// Creation Date:
// 2021-07-23 9:23 AM

#endregion

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Interfaces;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;

#endregion

namespace AhkScriptRunner
{
  internal class ConsoleClient : ConsoleWorker
  {
    private readonly AppConfig _appConfig;
    private readonly ILogger<ConsoleClient> _logger;
    private readonly ITwitchAPI _twitchApi;
    private readonly ITwitchPubSub _twitchClient;
    private User _userData;

    public ConsoleClient(IHostApplicationLifetime applicationLifetime, AppConfig appConfig,
      ITwitchAPI twitchApi, ITwitchPubSub twitchClient, ILogger<ConsoleClient> logger)
      : base(applicationLifetime)
    {
      _appConfig = appConfig;
      _twitchApi = twitchApi;
      _twitchClient = twitchClient;
      _logger = logger;

      _twitchApi.Settings.ClientId = _appConfig.ApplicationClientId;
      _twitchApi.Settings.Secret = _appConfig.ApplicationClientSecret;

      ConfigureListenEvents();
    }

    private async void ConfigureListenEvents()
    {
      _userData = (await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> {_appConfig.ChannelName}))
        .Users
        .FirstOrDefault();

      if (_userData is null)
      {
        _logger.LogCritical(
          $"{nameof(_appConfig.ChannelName)} with value '{_appConfig.ChannelName}' cannot be found on Twitch");
        Environment.Exit(-1);
        return;
      }

      _twitchClient.ListenToBitsEvents(_userData.Id);
      _twitchClient.ListenToRewards(_userData.Id);
      _twitchClient.OnPubSubServiceConnected += TwitchClientOnServiceConnected;
      _twitchClient.OnListenResponse += TwitchClientOnListenResponse;
      _twitchClient.OnRewardRedeemed += TwitchClientOnRewardRedeemed;
    }

    private void TwitchClientOnRewardRedeemed(object? sender, OnRewardRedeemedArgs e)
    {
      _logger.LogInformation($"Reward redeemed: {e.RedemptionId} - {e.RewardTitle}");
    }

    private void TwitchClientOnListenResponse(object? sender, OnListenResponseArgs e)
    {
      if (!e.Successful)
      {
        _logger.LogError($"{e.Response.Error}: {e.Topic}");
      }
    }

    private void TwitchClientOnServiceConnected(object? sender, EventArgs e)
    {
      _twitchClient.SendTopics(_appConfig.AccessToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Connecting PubSub client");
      _twitchClient.Connect();
      _logger.LogInformation("Connected PubSub client");

      return Task.CompletedTask;
    }

    protected override void OnStopping()
    {
      base.OnStopping();

      _logger.LogInformation("Disconnecting PubSub client");
      _twitchClient.Disconnect();
      _logger.LogInformation("Disconnecting PubSub client");
    }
  }
}