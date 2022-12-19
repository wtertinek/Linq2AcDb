using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Linq2Acad
{
  /// <summary>
  /// A container class that provides access to the elements of the Layer table. In addition to the standard LINQ operations this class provides methods to create, add and import LayerTableRecords.
  /// </summary>
  public sealed class LayerContainer : UniqueNameSymbolTableEnumerable<LayerTableRecord>
  {
    internal LayerContainer(Database database, Transaction transaction)
      : base(database, transaction, database.LayerTableId)
    {
    }

    /// <summary>
    /// Creates a new LayerTableRecord with the specified name and adds the Entities to it.
    /// </summary>
    /// <param name="name">The name of the new LayerTableRecord.</param>
    /// <param name="entities">The Entities that should be added to the new LayerTableRecord.</param>
    /// <returns>A new instance of LayerTableRecord.</returns>
    public LayerTableRecord Create(string name, IEnumerable<Entity> entities)
    {
      Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
      Require.TransactionNotDisposed(transaction.IsDisposed);
      Require.IsValidSymbolName(name, nameof(name));
      Require.NameDoesNotExist<LayerTableRecord>(Contains(name), name);
      Require.ElementsNotNull(entities, nameof(entities));

      var layer = CreateInternal(name);

      foreach (var entity in entities.UpgradeOpen())
      {
        entity.LayerId = layer.ObjectId;
      }

      return layer;
    }
  }
}
