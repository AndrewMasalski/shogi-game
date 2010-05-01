using DotUsi.Options.Base;
using DotUsi.SearchModifiers;

namespace DotUsi
{
  /// <summary>Represents modes <see cref="UsiEngine"/> can be in</summary>
  public enum EngineMode
  {
    /// <summary>Engine has just started. Most of properties are not initialized yet. </summary>
    /// <remarks>Call <see cref="UsiEngine.Usi"/> to initialize</remarks>
    Started,
    /// <summary>'usi' command has been sent to engine. No 'usiok' is received yet.</summary>
    Usi,
    /// <summary>Option's <see cref="ValueOptionBase{T}.Value"/> is changed 
    ///   or <see cref="UsiEngine.NewGame"/> method is called and <see cref="UsiEngine.IsReady"/> is not.
    /// </summary>
    Corrupted,
    /// <summary>Engine <see cref="UsiEngine.IsReady"/> method is called.</summary>
    Waiting, 
    /// <summary>1) Engine has been initialized with <see cref="UsiEngine.Usi"/> 
    ///   method and no operation is processing at the moment.
    /// 2) Search process started with <see cref="UsiEngine.Go"/> method is done</summary>
    Ready, 
    /// <summary>Engine's <see cref="UsiEngine.Go"/> method is called. 
    ///   When search finishes you will have <see cref="UsiEngine.BestMove"/> event raised.</summary>
    Searching,
    /// <summary>Engine's <see cref="UsiEngine.Go"/> method is called with <see cref="PonderModifier"/>. 
    ///   You can either stop it with <see cref="UsiEngine.Stop"/> if user didn't make "ponder move",
    ///   or with <see cref="UsiEngine.PonderHit"/> otherwise. 
    ///   Anyway ponder don't fire <see cref="UsiEngine.BestMove"/> event</summary>
    Pondering,
    /// <summary>Engine has been disposed</summary>
    Disposed
  }
}