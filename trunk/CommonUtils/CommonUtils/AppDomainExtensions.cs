using System;

namespace Yasc.Utils
{
  public static class AppDomainExtensions
  {
    public static T CreateInstanceAndUnwrap<T>(this AppDomain domain)
    {
      var assemblyName = typeof(T).Assembly.FullName;
      
      if (assemblyName == null)
        throw new ApplicationException(
          "Couldn't get assembly name for type " + typeof(T).FullName);

      return (T) domain.CreateInstanceAndUnwrap(assemblyName, typeof(T).FullName);
    }
  }
}