using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MainModule.Utils
{
  /// <summary>Lvs = LastVisitedServer</summary>
  internal static class LvsSettingsExtension
  {
    /// <summary>Lvs = LastVisitedServer</summary>
    public static IEnumerable<string> LoadLvs(this SettingsBase settings)
    {
      if (settings == null) throw new ArgumentNullException("settings");
      return LoadLvsInternal(settings).Distinct();
    }

    private static IEnumerable<string> LoadLvsInternal(SettingsBase settings)
    {
      for (int i = 1; i <= 10; i++)
      {
        var lvs = (string)settings["LastVisitedServer" + i];
        if (!string.IsNullOrEmpty(lvs)) yield return lvs;
      }
    }

    /// <summary>Saves Last Visited Servers list to the settings</summary>
    /// <param name="settings">Settings to save to</param>
    /// <param name="data">Last visited servers list</param>
    public static void SaveLvs(this SettingsBase settings, IList<string> data)
    {
      int lim = Math.Min(10, data.Count);
      for (int i = 0; i < lim; i++)
        settings["LastVisitedServer" + (i + 1)] = data[i];

      for (int i = lim; i < 10; i++)
        settings["LastVisitedServer" + (i + 1)] = null;
    }
  }
}