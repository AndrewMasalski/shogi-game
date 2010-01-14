namespace TestStand.WelcomeView
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new StandViewModel();
    }
  }
}
