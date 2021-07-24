#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ApiListeningEventHelper.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-24 11:09 AM

#endregion

#region usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllOverIt.Extensions;

#endregion

namespace TwitchScriptRunner.Helpers
{
  internal static class ApiListeningEventHelper
  {
    public static IReadOnlyCollection<string> GetAllEventNames()
    {
      return typeof(ApiListeningEvents).GetProperties()
        .Where(p => p.GetCustomAttributes(typeof(ChannelListenEventAttribute), false).Any())
        .Select(p => p.Name)
        .AsReadOnlyCollection();
    }

    public static IReadOnlyCollection<string> GetAllEventScriptPaths(string parentDir)
    {
      return GetAllEventNames().Select(p => Path.Combine(parentDir, p))
        .AsReadOnlyCollection();
    }
  }
}