namespace DotUsi
{
  /// <summary>Represents modes <see cref="UsiEngine"/> can be in</summary>
  public enum EngineMode
  {
    /// <summary>Engin has just started. Most of properties are not initialized yet. </summary>
    /// <remarks>Call <see cref="UsiEngine.Usi"/> to initialize</remarks>
    Started,
    Usi,
    Corrupted,
    /// <summary>Engine <see cref="UsiEngine.Usi"/> or <see cref="UsiEngine.NewGame"/> method is called.
    ///   These methods could take a while to complete but they are different from 
    ///   engine's <see cref="UsiEngine.Go"/> method which is designed to perform long calculations.
    /// </summary>
    Waiting, 
    /// <summary>1) Engine has been initialized with <see cref="UsiEngine.Usi"/> 
    ///   method and no operation is processing at the moment.</summary>
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