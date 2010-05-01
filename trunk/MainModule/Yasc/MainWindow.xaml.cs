using System;

namespace Yasc
{
  public partial class MainWindow 
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }

    protected override void OnClosed(EventArgs e)
    {
      ((MainWindowViewModel) DataContext).Dispose();
      base.OnClosed(e);
    }
  }
}
