using System.Collections.Generic;
using DotUsi.Options.Base;
using DotUsi.Process;

namespace DotUsi.Drivers
{
  /// <summary>If <see cref="IUsiProcess"/> supports that interface
  ///   <see cref="UsiEngine"/> will pass additional info to it.
  /// </summary>
  /// <remarks>To be used with drivers</remarks>
  public interface IEngineHook
  {
    /// <summary>Engine gives reference to itself to the driver</summary>
    void SetEngine(UsiEngine engine);
    /// <summary>Engine extends its options set with the options this method returns</summary>
    IEnumerable<UsiOptionBase> GetImplicitOptions();
  }
}