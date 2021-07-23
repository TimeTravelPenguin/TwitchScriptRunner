#region Title Header

// Name: Phillip Smith
// 
// Solution: AhkScriptRunner
// Project: AhkScriptRunner
// File Name: AppConfig.cs
// 
// Current Data:
// 2021-07-23 11:19 AM
// 
// Creation Date:
// 2021-07-23 9:18 AM

#endregion

namespace AhkScriptRunner
{
  internal class AppConfig
  {
    public string ApplicationClientId { get; set; }
    public string ApplicationClientSecret { get; set; }
    public string ChannelName { get; set; }
    public string AccessToken { get; set; }
  }
}