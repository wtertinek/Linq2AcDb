﻿using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad
{
  /// <summary>
  /// Represents the result if an import operation.
  /// </summary>
  /// <typeparam name="T">The type of the object that has been imported.</typeparam>
  public class ImportResult<T> where T : DBObject
  {
    internal ImportResult(T element, IdMapping mapping)
    {
      Item = element;
      Mapping = mapping.Cast<IdPair>()
                       .ToDictionary(p => p.Key, p => p.Value);
    }

    /// <summary>
    /// The item that has been imported.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// ID mapping from objects in the source database to objects in the target database.
    /// </summary>
    public Dictionary<ObjectId, ObjectId> Mapping { get; }
  }
}
