using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Yasc.ShogiCore.Utils
{
  /// <summary>Provides convenient way to support for property change notifications. </summary>
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    #region ' Debugging Aides '

    /// <summary>
    /// Returns whether an exception is thrown, or if a Debug.Fail() is used
    /// when an invalid property name is passed to the VerifyPropertyName method.
    /// The default value is false, but subclasses used by unit tests might 
    /// override this property's getter to return true.
    /// </summary>
    protected virtual bool ThrowOnInvalidPropertyName { get { return true; } }

    /// <summary>
    /// Warns the developer if this object does not have
    /// a public property with the specified name. This 
    /// method does not exist in a Release build.
    /// </summary>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    private void VerifyPropertyName(string propertyName)
    {
      // Verify that the property name matches a real,  
      // public, instance property on this object.
      if (TypeDescriptor.GetProperties(this)[propertyName] != null)
        return;
      var msg = "Invalid property name: " + propertyName;
      if (ThrowOnInvalidPropertyName)
        throw new Exception(msg);
      Debug.Fail(msg);
    }

#if DEBUG
    /// <summary>
    /// Useful for ensuring that ViewModel objects are properly garbage collected.
    /// </summary>
    ~ViewModelBase()
    {
      string msg = string.Format(
        "{0} ({1}) Finalized",
        GetType().Name, GetHashCode());
      Debug.WriteLine(msg);
    }
#endif

    #endregion

    #region ' NotifyPropertyChanged '

    /// <summary>Raised when a property on this object has a new value.</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Raises this object's PropertyChanged event.</summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      VerifyPropertyName(propertyName);

      var handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}