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
  /// A container class that provides access to the elements of the MLeaderStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import MLeaderStyles.
  /// </summary>
  public sealed class MLeaderStyleContainer : DBDictionaryEnumerable<MLeaderStyle>
  {
    internal MLeaderStyleContainer(Database database, Transaction transaction, ObjectId containerID)
      : base(database, transaction, containerID, s => s.Name, () => nameof(MLeaderStyle.Name))
    {
    }
  }
}
