namespace DotUsi
{
  public enum EngineMode
  {
    /// <summary>Engin has just started. Most of properties are not initialized yet. </summary>
    /// <remarks>Call <see cref="UsiEngine.Usi"/> to initialize</remarks>
    Started, 
    /// <summary>1) Engine has just been initialized or ist's finished operation and ready
    /// 2) You've requested some operation which might take long time 
    ///    (set options, start new game, etc.) but you didn't call IsReady()
    /// TODO: Let's push it into Processing state for the 2nd case
    /// </summary>
    Ready, 
    /// <summary>IsReady is called and no confirmation has been received yet</summary>
    Processing, Searching, Pondering
  }
}