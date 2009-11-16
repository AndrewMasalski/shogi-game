using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace Yasc.Gui
{
  public static class LvsSettingsExtension
  {
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
    private static void SaveLvs(SettingsBase settings, IList<string> data)
    {
      int lim = Math.Min(10, data.Count);
      for (int i = 0; i < lim; i++)
        settings["LastVisitedServer" + (i + 1)] = data[i];
    }

    public static void SaveLvs(this SettingsBase settings, ObservableCollection<string> lvs, string address)
    {
      int idx = lvs.IndexOf(address);
      if (idx != -1)
      {
        lvs.Move(idx, 0);
      }
      else
      {
        lvs.Insert(0, address);
      }
      SaveLvs(settings, lvs);
    }    
  }
}