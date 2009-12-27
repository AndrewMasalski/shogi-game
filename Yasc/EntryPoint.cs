using System;
using System.IO;
using System.Windows;

namespace Yasc
{
  class EntryPoint
  {
    /// <summary>Application Entry Point.</summary>
    [STAThreadAttribute]
    public static void Main()
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
      // Extracting everything else into separate function guaranties that 
      // UnhandledException subscription happens before any "I couldn't load 
      // that WPF assembly" exception.
      StartApp();
    }
    
    private static void StartApp()
    {
      var app = new App();
      app.InitializeComponent();
      app.Run();
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
      try
      {
        const string myMessage = "Unfortunately something unexpected happend. "+
          "Could you copy this text (Ctrl+C) and leave it as a comment to "+
          "any wiki page you find the most appropriate here: "+
          "http://code.google.com/p/shogi-game/w/list. Please.\r\n\r\n";

        string exMessage = args.ExceptionObject.ToString();
        MessageBox.Show(myMessage + exMessage, "Unhandled exception", 
          MessageBoxButton.OK, MessageBoxImage.Error);
      }
      catch (Exception x)
      {
        Console.WriteLine(x);
        // That's the last hope
        SaveToFile(args.ExceptionObject);
      }
    }

    private static void SaveToFile(object exception)
    {
      try
      {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        File.AppendAllText(Path.Combine(path, "ShogiGameError.log"), exception.ToString());
      }
      catch (Exception x)
      {
        Console.WriteLine(x);
      }
    }
  }
}
