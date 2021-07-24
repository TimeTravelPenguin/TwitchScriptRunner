#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ChannelListenEventAttribute.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-24 11:03 AM

#endregion

#region usings

using System;

#endregion

namespace TwitchScriptRunner
{
  [AttributeUsage(AttributeTargets.Property)]
  internal class ChannelListenEventAttribute : Attribute
  {
  }
}