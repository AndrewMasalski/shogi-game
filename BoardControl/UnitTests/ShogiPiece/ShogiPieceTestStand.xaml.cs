using System;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoardControl.UnitTests.ShogiPiece
{
  public partial class ShogiPieceTestStand 
  {
    public ShogiPieceTestStand()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      _contentControl.Content = new ShogiPieceTestControl();
    }
  }

  [TestClass]
  public class A
  {
    [TestMethod, Ignore]
    public void Run()
    {
      Exception ex = null;
      var thread = new Thread(() =>
          {
            var application = new Application();
            application.DispatcherUnhandledException +=(s, e) =>
                {
                  ex = e.Exception;
                  e.Handled = true;
                };
            application.Run(new ShogiPieceTestStand());
          });
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      while (thread.ThreadState == ThreadState.Running)
      {
        if (ex != null)
          Assert.Fail(ex.ToString());
        Thread.Sleep(200);
      }
    }
  }

}
