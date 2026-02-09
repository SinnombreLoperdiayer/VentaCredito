using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Clase para implementar comparaciones a traves de expresiones Lambda
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class COLambdaComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> _lambdaComparer;
    private readonly Func<T, int> _lambdaHash;

    public COLambdaComparer(Func<T, T, bool> lambdaComparer) :
      this(lambdaComparer, o => 0)
    {
    }

    public COLambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
    {
      if (lambdaComparer == null)
        throw new ArgumentNullException("lambdaComparer");
      if (lambdaHash == null)
        throw new ArgumentNullException("lambdaHash");

      _lambdaComparer = lambdaComparer;
      _lambdaHash = lambdaHash;
    }

    public bool Equals(T x, T y)
    {
      return _lambdaComparer(x, y);
    }

    public int GetHashCode(T obj)
    {
      return _lambdaHash(obj);
    }
  }
}