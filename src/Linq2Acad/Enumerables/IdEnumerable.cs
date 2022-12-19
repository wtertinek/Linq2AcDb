﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad
{
  // This code is based on the implementation of System.Linq.Enumerable
  // http://referencesource.microsoft.com/#system.core/System/Linq/Enumerable.cs

  #region Internal base class IdEnumerable

  internal abstract class IdEnumerable<T> : IEnumerable<T>
  {
    [DebuggerStepThrough]
    protected IdEnumerable()
    {
    }

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    public abstract IdEnumerable<T> Concat(IEnumerable<T> second);

    public abstract int Count();

    public abstract T ElementAt(int index);

    public abstract T ElementAtOrDefault(int index);

    public abstract T Last();

    public abstract T LastOrDefault();

    public abstract long LongCount();

    public abstract IEnumerable<T> Skip(int count);
  }

  #endregion

  #region Internal class LazayIdEnumerable

  internal class LazayIdEnumerable<T> : IdEnumerable<T>
  {
    internal readonly IEnumerable ids;
    internal readonly Func<object, T> getID;

    public LazayIdEnumerable(IEnumerable ids, Func<object, T> getID)
    {
      this.ids = ids;
      this.getID = getID;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      var enumerator = ids.GetEnumerator();

      while (enumerator.MoveNext())
      {
        yield return getID(enumerator.Current);
      }
    }

    public override IdEnumerable<T> Concat(IEnumerable<T> second)
    {
      if (second is LazayIdEnumerable<T>)
      {
        var other = second as LazayIdEnumerable<T>;
        return new ConcatIdEnumerable<T>(Tuple.Create(ids, getID), Tuple.Create(other.ids, other.getID));
      }
      else if (second is ConcatIdEnumerable<T>)
      {
        return new ConcatIdEnumerable<T>(new[] { Tuple.Create(ids, getID) }.Concat((second as ConcatIdEnumerable<T>).ids).ToArray());
      }
      else
      {
        return new ConcatIdEnumerable<T>(Tuple.Create(ids, getID),
                                         Tuple.Create((IEnumerable)second, (Func<object, T>)(id => (T)id)));
      }
    }

    public override int Count()
    {
      var count = 0;
      var enumerator = ids.GetEnumerator();

      while (enumerator.MoveNext())
      {
        count++;
      }

      return count;
    }

    public override T ElementAt(int index)
    {
      var enumerator = ids.GetEnumerator();

      while (true)
      {
        if (!enumerator.MoveNext())
        {
          throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index == 0)
        {
          return getID(enumerator.Current);
        }

        index--;
      }
    }

    public override T ElementAtOrDefault(int index)
    {
      var enumerator = ids.GetEnumerator();

      while (true)
      {
        if (!enumerator.MoveNext())
        {
          break;
        }

        if (index == 0)
        {
          return getID(enumerator.Current);
        }

        index--;
      }

      return default;
    }

    public override T Last()
    {
      var enumerator = ids.GetEnumerator();

      if (enumerator.MoveNext())
      {
        object result;

        do
        {
          result = enumerator.Current;
        }
        while (enumerator.MoveNext());

        return getID(result);
      }
      else
      {
        throw new InvalidOperationException("Sequence contains no elements");
      }
    }

    public override T LastOrDefault()
    {
      var enumerator = ids.GetEnumerator();

      if (enumerator.MoveNext())
      {
        object result;

        do
        {
          result = enumerator.Current;
        }
        while (enumerator.MoveNext());

        return getID(result);
      }
      else
      {
        return default;
      }
    }

    public override long LongCount()
    {
      long count = 0;
      var enumerator = ids.GetEnumerator();

      while (enumerator.MoveNext())
      {
        count++;
      }

      return count;
    }

    public override IEnumerable<T> Skip(int count)
    {
      var enumerable = ids.GetEnumerator();

      while (count > 0 && enumerable.MoveNext())
      {
        count--;
      }

      if (count <= 0)
      {
        while (enumerable.MoveNext())
        {
          yield return getID(enumerable.Current);
        }
      }
    }
  }

  #endregion

  #region Internal class MaterializedIdEnumerable

  internal class MaterializedIdEnumerable<T> : IdEnumerable<T>
  {
    private readonly IEnumerable<T> ids;

    public MaterializedIdEnumerable(IEnumerable<T> ids)
      => this.ids = ids;

    public override IEnumerator<T> GetEnumerator()
      => ids.GetEnumerator();

    public override IdEnumerable<T> Concat(IEnumerable<T> second)
      => new MaterializedIdEnumerable<T>(ids.Concat(second));

    public override int Count()
      => ids.Count();

    public override T ElementAt(int index)
      => ids.ElementAt(index);

    public override T ElementAtOrDefault(int index)
      => ids.ElementAtOrDefault(index);

    public override T Last()
      => ids.Last();

    public override T LastOrDefault()
      => ids.LastOrDefault();

    public override long LongCount()
      => ids.LongCount();

    public override IEnumerable<T> Skip(int count)
      => ids.Skip(count);
  }

  #endregion

  #region Internal class ConcatIdEnumerable

  internal class ConcatIdEnumerable<T> : IdEnumerable<T>
  {
    internal readonly Tuple<IEnumerable, Func<object, T>>[] ids;

    public ConcatIdEnumerable(params Tuple<IEnumerable, Func<object, T>>[] ids)
      => this.ids = ids;

    public override IEnumerator<T> GetEnumerator()
    {
      foreach (var id in ids)
      {
        var enumerator = id.Item1.GetEnumerator();

        while (enumerator.MoveNext())
        {
          yield return id.Item2(enumerator.Current);
        }
      }
    }

    public override IdEnumerable<T> Concat(IEnumerable<T> second)
    {
      if (second is LazayIdEnumerable<T>)
      {
        var other = second as LazayIdEnumerable<T>;
        return new ConcatIdEnumerable<T>(ids.Concat(new [] { Tuple.Create(other.ids, other.getID) }).ToArray());
      }
      else if (second is ConcatIdEnumerable<T>)
      {
        return new ConcatIdEnumerable<T>(ids.Concat((second as ConcatIdEnumerable<T>).ids).ToArray());
      }
      else
      {
        var enumerable = second as IEnumerable;
        return new ConcatIdEnumerable<T>(ids.Concat(new[]
                                                    {
                                                      Tuple.Create(enumerable, new Func<object, T>(id => (T)id))
                                                    }).ToArray());
      }
    }

    public override int Count()
    {
      var count = 0;

      foreach (var id in ids)
      {
        var enumerator = id.Item1.GetEnumerator();

        while (enumerator.MoveNext())
        {
          count++;
        }
      }

      return count;
    }

    public override T ElementAt(int index)
    {
      for (int i = 0; i < ids.Length; i++)
      {
        var enumerator = ids[i].Item1.GetEnumerator();

        while (true)
        {
          if (!enumerator.MoveNext())
          {
            if (i == (ids.Length - 1))
            {
              throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
              break;
            }
          }

          if (index == 0)
          {
            return ids[i].Item2(enumerator.Current);
          }

          index--;
        }
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }

    public override T ElementAtOrDefault(int index)
    {
      for (int i = 0; i < ids.Length; i++)
      {
        var enumerator = ids[i].Item1.GetEnumerator();

        while (true)
        {
          if (!enumerator.MoveNext())
          {
            break;
          }

          if (index == 0)
          {
            return ids[i].Item2(enumerator.Current);
          }

          index--;
        }
      }

      return default;
    }

    public override T Last()
    {
      for (int i = ids.Length - 1; i >= 0; i--)
      {
        var enumerator = ids[i].Item1.GetEnumerator();

        if (enumerator.MoveNext())
        {
          object result;

          do
          {
            result = enumerator.Current;
          }
          while (enumerator.MoveNext());

          return ids[i].Item2(result);
        }
      }

      throw new InvalidOperationException("Sequence contains no elements");
    }

    public override T LastOrDefault()
    {
      for (int i = ids.Length - 1; i >= 0; i--)
      {
        var enumerator = ids[i].Item1.GetEnumerator();

        if (enumerator.MoveNext())
        {
          object result;

          do
          {
            result = enumerator.Current;
          }
          while (enumerator.MoveNext());

          return ids[i].Item2(result);
        }
      }

      return default;
    }

    public override long LongCount()
    {
      long count = 0;

      foreach (var id in ids)
      {
        var enumerator = id.Item1.GetEnumerator();

        while (enumerator.MoveNext())
        {
          count++;
        }
      }

      return count;
    }

    public override IEnumerable<T> Skip(int count)
    {
      for (int i = 0; i < ids.Length; i++)
      {
        var enumerable = ids[i].Item1.GetEnumerator();

        while (count > 0 && enumerable.MoveNext())
        {
          count--;
        }

        if (count <= 0)
        {
          break;
        }
      }

      if (count <= 0)
      {
        for (int i = 0; i < ids.Length; i++)
        {
          var enumerable = ids[i].Item1.GetEnumerator();
          
          while (enumerable.MoveNext())
          {
            yield return ids[i].Item2(enumerable.Current);
          }
        }
      }
    }
  }

  #endregion
}
