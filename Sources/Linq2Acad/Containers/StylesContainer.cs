﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Linq2Acad
{
  /// <summary>
  /// Provides access to all style related containers.
  /// </summary>
  public class StylesContainer
  {
    private readonly Database database;
    private readonly Transaction transaction;

    internal StylesContainer(Database database, Transaction transaction)
    {
      this.database = database;
      this.transaction = transaction;
    }

    /// <summary>
    /// Provides access to the elements of the DimStyle table. In addition to the standard LINQ operations this class provides methods to create, add and import DimStyles.
    /// </summary>
    public DimStyleContainer DimStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new DimStyleContainer(database, transaction);
      }
    }

    /// <summary>
    /// Provides access to the elements of the TextStyle table. In addition to the standard LINQ operations this class provides methods to create, add and import TextStyles.
    /// </summary>
    public TextStyleContainer TextStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new TextStyleContainer(database, transaction);
      }
    }

    /// <summary>
    /// Provides access to the elements of the MLeaderStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import MLeaderStyles.
    /// </summary>
    public MLeaderStyleContainer MLeaderStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new MLeaderStyleContainer(database, transaction, database.MLeaderStyleDictionaryId);
      }
    }

    /// <summary>
    /// Provides access to the elements of the MlineStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import MlineStyles.
    /// </summary>
    public MlineStyleContainer MlineStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new MlineStyleContainer(database, transaction, database.MLStyleDictionaryId);
      }
    }

    /// <summary>
    /// Provides access to the elements of the DBVisualStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import DBVisualStyles.
    /// </summary>
    public DBVisualStyleContainer DBVisualStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new DBVisualStyleContainer(database, transaction, database.VisualStyleDictionaryId);
      }
    }

    /// <summary>
    /// Provides access to the elements of the TableStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import TableStyles.
    /// </summary>
    public TableStyleContainer TableStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new TableStyleContainer(database, transaction, database.TableStyleDictionaryId);
      }
    }

    /// <summary>
    /// Provides access to the elements of the SectionViewStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import SectionViewStyles.
    /// </summary>
    public SectionViewStyleContainer SectionViewStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new SectionViewStyleContainer(database, transaction, database.SectionViewStyleDictionaryId);
      }
    }

    /// <summary>
    /// Provides access to the elements of the DetailViewStyle dictionary. In addition to the standard LINQ operations this class provides methods to create, add and import DetailViewStyles.
    /// </summary>
    public DetailViewStyleContainer DetailViewStyles
    {
      get
      {
        Require.NotDisposed(database.IsDisposed, nameof(AcadDatabase));
        return new DetailViewStyleContainer(database, transaction, database.DetailViewStyleDictionaryId);
      }
    }

    #region Overrides to remove methods from IntelliSense

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    #endregion
  }
}
