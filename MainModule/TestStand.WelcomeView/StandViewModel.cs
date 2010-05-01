using System;
using System.Collections.ObjectModel;
using Yasc.Gui;
using Yasc.Utils.Mvvm;

namespace TestStand.WelcomeView
{
  public class StandViewModel : ObservableObject
  {
    public WelcomeViewModel WelcomeViewModel { get; private set;}
    public ObservableCollection<StandEvent> EventsLog { get; private set; }

    public StandViewModel()
    {
      WelcomeViewModel = new WelcomeViewModel();
      WelcomeViewModel.ChoiceDone += OnChoiceDone;

      EventsLog = new ObservableCollection<StandEvent>();
    }

    private void OnChoiceDone(object sender, EventArgs args)
    {
      EventsLog.Add(new StandEvent("ChoiceDone"));
    }
  }
}