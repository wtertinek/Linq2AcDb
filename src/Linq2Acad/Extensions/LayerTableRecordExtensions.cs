using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad
{
  /// <summary>
  /// Extension methods for instances of LayerTableRecord.
  /// </summary>
  public static class LayerTableRecordExtensions
  {
    /// <summary>
    /// Adds the given entity to this layer.
    /// </summary>
    /// <param name="layer">The layer instance.</param>
    /// <param name="entity">The entity to add.</param>
    public static void Add(this LayerTableRecord layer, Entity entity)
    {
      Require.ParameterNotNull(layer, nameof(layer));
      Require.ParameterNotNull(entity, nameof(entity));

      AddInternal(layer, entity);
    }

    /// <summary>
    /// Adds the given entities to this layer.
    /// </summary>
    /// <param name="layer">The layer instance.</param>
    /// <param name="entities">The entities to add.</param>
    public static void AddRange(this LayerTableRecord layer, IEnumerable<Entity> entities)
    {
      Require.ParameterNotNull(layer, nameof(layer));
      Require.ParameterNotNull(entities, nameof(entities));

      foreach (var entity in entities)
      {
        AddInternal(layer, entity);
      }
    }

    /// <summary>
    /// Adds the given entity to this layer.
    /// </summary>
    /// <param name="layer">The layer instance.</param>
    /// <param name="entity">The entity to add.</param>
    private static void AddInternal(LayerTableRecord layer, Entity entity)
      => Helpers.WriteWrap(entity, () => entity.LayerId = layer.ObjectId);
  }
}
