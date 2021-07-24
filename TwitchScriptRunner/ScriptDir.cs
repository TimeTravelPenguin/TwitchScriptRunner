#region Title Header

// Name: Phillip Smith
// 
// Solution: TwitchScriptRunner
// Project: TwitchScriptRunner
// File Name: ScriptDir.cs
// 
// Current Data:
// 2021-07-24 3:17 PM
// 
// Creation Date:
// 2021-07-24 10:21 AM

#endregion

#region usings

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace TwitchScriptRunner
{
  public class ScriptDir
  {
    public string ScriptDirParentPath { get; set; }

    [JsonIgnore]
    public IEnumerable<string> EventScriptDirectories { get; set; } = new List<string>();
  }
}