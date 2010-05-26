using System;
using System.Runtime.Serialization;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>This exception is thrown when you are trying 
  ///   to make invalid move (<see cref="Board.MakeMove"/>)</summary>
  [Serializable]
  public sealed class InvalidMoveException : Exception
  {
    ///<summary>Violation date</summary>
    public RulesViolation ViolatedRule { get; set; }

    /// <summary>ctor</summary>
    public InvalidMoveException(RulesViolation violatedRule) 
    {
      ViolatedRule = violatedRule;
    }
  
    private InvalidMoveException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
    {
      ViolatedRule = (RulesViolation)info.GetValue("ViolatedRule", typeof(RulesViolation));
    }

    /// <summary>Partivipates serialization process</summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ViolatedRule", ViolatedRule);
    }
  }
}