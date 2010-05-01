using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Yasc.BoardControl.Common
{
  public class StopwatchControl : Control
  {
    private TimeSpan _offset;
    private readonly DispatcherTimer _timer;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    static StopwatchControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StopwatchControl),
        new FrameworkPropertyMetadata(typeof(StopwatchControl)));
    }
    public StopwatchControl()
    {
      _timer = new DispatcherTimer { IsEnabled = false };
      _timer.Tick += TimerOnTick;

      CommandManager.AddCanExecuteHandler(this, CanExecute);
      CommandManager.AddExecutedHandler(this, Executed);
    }

    #region ' Value '

    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(TimeSpan),
      typeof(StopwatchControl), new UIPropertyMetadata(TimeSpan.Zero, ValuePropertyChanged));

    private static void ValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((StopwatchControl)o).ValuePropertyChanged((TimeSpan)args.NewValue);
    }

    private void ValuePropertyChanged(TimeSpan value)
    {
      _offset = Direction == TimerDirection.Forward ? value - _stopwatch.Elapsed : value + _stopwatch.Elapsed;
    }

    public TimeSpan Value
    {
      get { return (TimeSpan)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    #endregion

    #region ' StopValue '

    public static readonly DependencyProperty StopValueProperty =
      DependencyProperty.Register("StopValue", typeof(TimeSpan),
      typeof(StopwatchControl), new UIPropertyMetadata(TimeSpan.Zero, StopValuePropertyChanged));

    private static void StopValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((StopwatchControl)o).StopValuePropertyChanged((TimeSpan)args.NewValue);
    }

    private void StopValuePropertyChanged(TimeSpan value)
    {
      _offset = Direction == TimerDirection.Forward ? value - _stopwatch.Elapsed : value + _stopwatch.Elapsed;
    }

    public TimeSpan StopValue
    {
      get { return (TimeSpan)GetValue(StopValueProperty); }
      set { SetValue(StopValueProperty, value); }
    }

    #endregion

    #region ' IsLaunched '

    public static readonly DependencyProperty IsLaunchedProperty =
      DependencyProperty.Register("IsLaunched", typeof(bool),
      typeof(StopwatchControl), new UIPropertyMetadata(false, IsLaunchedPropertyChanged));

    private static void IsLaunchedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StopwatchControl)d).IsLaunchedPropertyChanged((bool)e.NewValue);
    }

    private void IsLaunchedPropertyChanged(bool newValue)
    {
      if (newValue)
        _stopwatch.Start();
      else
        _stopwatch.Stop();
      _timer.IsEnabled = newValue;
    }

    public bool IsLaunched
    {
      get { return (bool)GetValue(IsLaunchedProperty); }
      set { SetValue(IsLaunchedProperty, value); }
    }

    #endregion

    #region ' RefreshInterval '

    public static readonly DependencyProperty RefreshIntervalProperty =
      DependencyProperty.Register("RefreshInterval", typeof(TimeSpan), typeof(StopwatchControl),
      new UIPropertyMetadata(TimeSpan.FromSeconds(.1), RefreshIntervalPropertyChanged));

    private static void RefreshIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StopwatchControl)d)._timer.Interval = (TimeSpan)e.NewValue;
    }

    public TimeSpan RefreshInterval
    {
      get { return (TimeSpan)GetValue(RefreshIntervalProperty); }
      set { SetValue(RefreshIntervalProperty, value); }
    }

    #endregion

    #region ' Direction '

    public static readonly DependencyProperty DirectionProperty =
      DependencyProperty.Register("Direction", typeof(TimerDirection), typeof(StopwatchControl),
      new UIPropertyMetadata(TimerDirection.Forward, DirectionPropertyChanged));

    private static void DirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StopwatchControl)d).DirectionPropertyChanged((TimerDirection)e.OldValue);
    }

    private void DirectionPropertyChanged(TimerDirection oldDirection)
    {
      if (IsLaunched)
      {
        _stopwatch.Stop();
      }

      _offset += oldDirection == TimerDirection.Forward ? 
        _stopwatch.Elapsed : TimeSpan.Zero - _stopwatch.Elapsed;

      if (IsLaunched)
      {
        _stopwatch.Reset();
        _stopwatch.Start();
      }
    }

    public TimerDirection Direction
    {
      get { return (TimerDirection)GetValue(DirectionProperty); }
      set { SetValue(DirectionProperty, value); }
    }

    #endregion

    #region ' Commands '

    public static readonly RoutedUICommand StartCommand
      = new RoutedUICommand("Start", "StartCommand", typeof(StopwatchControl));

    public static readonly RoutedUICommand StopCommand
      = new RoutedUICommand("Stop", "StopCommand", typeof(StopwatchControl));

    public static readonly RoutedUICommand FlipCommand
      = new RoutedUICommand("Flip", "FlipCommand", typeof(StopwatchControl));

    public static readonly RoutedUICommand ResetCommand
      = new RoutedUICommand("Reset", "ResetCommand", typeof(StopwatchControl));

    private void Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Command == StartCommand)
      {
        IsLaunched = true;
      }
      else if (e.Command == StopCommand)
      {
        IsLaunched = false;
      }
      else if (e.Command == FlipCommand)
      {
        IsLaunched = !IsLaunched;
      }
      else if (e.Command == ResetCommand)
      {
        Value = TimeSpan.Zero;
      }
    }
    private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (e.Command == StartCommand)
      {
        e.CanExecute = !IsLaunched;
        e.ContinueRouting = false;
      }
      else if (e.Command == StopCommand)
      {
        e.CanExecute = IsLaunched;
        e.ContinueRouting = false;
      }
      else if (e.Command == FlipCommand)
      {
        e.CanExecute = true;
        e.ContinueRouting = false;
      }
      else if (e.Command == ResetCommand)
      {
        e.CanExecute = true;
        e.ContinueRouting = false;
      }
      else
      {
        e.CanExecute = false;
        e.ContinueRouting = true;
      }
    }

    #endregion

    private void TimerOnTick(object sender, EventArgs args)
    {
      bool before = Value < StopValue;
      TimeSpan value = Direction != TimerDirection.Forward ?
        _offset - _stopwatch.Elapsed : _offset + _stopwatch.Elapsed;
      bool after = value < StopValue;
      if (before == after)
      {
        Value = value;
      }
      else
      {
        Value = StopValue;
        IsLaunched = false;
      }
    }
  }
}