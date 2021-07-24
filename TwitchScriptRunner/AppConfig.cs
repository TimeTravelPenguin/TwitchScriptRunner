#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: AppConfig.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-23 9:18 AM

#endregion

#region usings

#endregion

namespace TwitchScriptRunner
{
  internal class AppConfig
  {
    public string ApplicationClientId { get; set; }
    public string ApplicationClientSecret { get; set; }
    public string ChannelName { get; set; }
    public string OAuthAccessToken { get; set; }

    public ApiListeningEvents ApiListeningEvents { get; set; } = new();

    public ScriptDir ScriptDirectory { get; set; } = new();
  }
}