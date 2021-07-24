#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ApiListeningEvents.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-24 11:03 AM

#endregion

namespace TwitchScriptRunner
{
  internal class ApiListeningEvents
  {
    [ChannelListenEvent]
    public bool BitDonations { get; set; }

    [ChannelListenEvent]
    public bool ChannelPointRedemption { get; set; }
  }
}