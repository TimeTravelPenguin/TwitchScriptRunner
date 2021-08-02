#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: AppConfig.cs
// 
// Current Data:
// 2021-08-02 10:27 AM
// 
// Creation Date:
// 2021-07-23 9:18 AM

#endregion

#region usings

using System;

#endregion

namespace TwitchScriptRunner
{
  internal class AppConfig
  {
    public ApiListeningEvents ApiListeningEvents { get; set; } = new();
    public string ApplicationClientId { get; set; }
    public string ApplicationClientSecret { get; set; }
    public string ChannelName { get; set; }
    public DateTime NextTokenRefresh { get; set; } = DateTime.Today;
    public string OAuthAccessToken { get; set; }
    public string OAuthRefreshToken { get; set; }
    public ScriptDir ScriptDirectory { get; set; } = new();
  }
}