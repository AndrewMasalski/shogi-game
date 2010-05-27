using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtils.UnitTests
{
  public static class MyAssert
  {
    public static void ThrowsException<TException>(Action action)
      where TException : Exception
    {
      try
      {
        action();
      }
      catch (Exception x)
      {
        Assert.AreEqual(typeof(TException), x.GetType());
        return;
      }
      Assert.Fail("Exception's been expected: " + typeof(TException));
    }
  }
}