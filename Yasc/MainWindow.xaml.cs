using System;
using System.Windows;
using System.Windows.Input;
using Yasc.Controls;
using Yasc.Networking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using Yasc.GenericDragDrop;
using Yasc.Utils;

namespace Yasc
{
  public partial class MainWindow 
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }
  }
}
