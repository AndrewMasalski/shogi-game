using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class UserControl1 : UserControl
  {
    public UserControl1()
    {
      InitializeComponent();
    }

    protected override Size MeasureOverride(Size constraint)
    {
      _measure.Text = string.Format("{0:f0} x {1:f0}", constraint.Width, constraint.Height);
      _arrange.Text = string.Format("{0} x {1}", constraint.Width, constraint.Height);
      return base.MeasureOverride(constraint);
    }
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      _arrange.Text = string.Format("{0:f0} x {1:f0}", arrangeBounds.Width, arrangeBounds.Height);
      return base.ArrangeOverride(arrangeBounds);
    }
  }
}
