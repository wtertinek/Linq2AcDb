﻿using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad
{
  /// <summary>
  /// The main class that provides access to the drawing database.
  /// </summary>
  public sealed class AcadDatabase : AcadDataModel, IDisposable
  {
    private readonly bool commitTransaction;
    private readonly bool disposeTransaction;
    private readonly bool disposeDatabase;
    private readonly bool saveOnCommit;
    private readonly string saveAsFileName;
    private readonly SaveAsDwgVersion dwgVersion;

    private AcadDatabase(Database database, bool keepOpen, string outFileName, SaveAsDwgVersion dwgVersion)
      : this(database, database.TransactionManager.StartTransaction(), true, true)
    {
      saveOnCommit = outFileName != null;
      saveAsFileName = outFileName;
      this.dwgVersion = dwgVersion;
      disposeDatabase = !keepOpen;
    }

    private AcadDatabase(Database database, bool keepOpen)
      : this(database, database.TransactionManager.StartTransaction(), true, true)
    {
      disposeDatabase = !keepOpen;
      saveOnCommit = false;
    }

    private AcadDatabase(Database database, Transaction transaction, bool commitTransaction, bool disposeTransaction)
      : base(database, transaction)
    {
      this.commitTransaction = commitTransaction;
      this.disposeTransaction = disposeTransaction;
    }

    #region Instance methods

    /// <summary>
    /// Immediately discards all changes and aborts the underlying transaction. The session is no longer usable after calling this method.
    /// </summary>
    public void Abort()
    {
      if (!transaction.IsDisposed)
      {
        transaction.Abort();
        transaction.Dispose();
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (!transaction.IsDisposed)
      {
        Require.NotDisposed(Database.IsDisposed, nameof(AcadDatabase));

        if (commitTransaction)
        {
          transaction.Commit();

          if (SummaryInfo.Changed)
          {
            SummaryInfo.Commit();
          }

          if (saveOnCommit)
          {
            Database.SaveAs(saveAsFileName, GetDwgVersion());
          }
        }

        if (disposeTransaction)
        {
          transaction.Dispose();
        }
      }

      if (disposeDatabase)
      {
        Database.Dispose();
      }
    }

    /// <summary>
    /// Convert the SaveAsDwgVersion enum to Autodesk's DwgVersion.
    /// </summary>
    /// <returns>Autodesk DwgVersion.</returns>
    private DwgVersion GetDwgVersion()
    {
      switch (dwgVersion)
      {
        case SaveAsDwgVersion.DWG2004:
          return DwgVersion.AC1015;
        case SaveAsDwgVersion.DWG2007:
          return DwgVersion.AC1021;
        case SaveAsDwgVersion.DWG2010:
          return DwgVersion.AC1024;
        case SaveAsDwgVersion.DWG2013:
          return DwgVersion.AC1027;
#if AutoCAD_2018 || AutoCAD_2019 || AutoCAD_2020 || AutoCAD_2021 || AutoCAD_2022 || AutoCAD_2023 || AutoCAD_2024
        case SaveAsDwgVersion.DWG2018:
          return DwgVersion.AC1032;
#endif
        case SaveAsDwgVersion.NewestAvailable:
          return DwgVersion.Newest;
        case SaveAsDwgVersion.DontChange:
        default:
          return Database.OriginalFileSavedByVersion;
      }
    }

    #endregion

    #region Factory methods

    /// <summary>
    /// Provides access to a newly created drawing database.
    /// </summary>
    /// <param name="options">Database create options.</param>
    /// <returns>The AcadDatabase instance.</returns>
    public static AcadDatabase Create(CreateOptions options = null)
    {
      if (options == null)
      {
        options = new CreateOptions()
                  {
                    KeepDatabaseOpen = false,
                    SaveDwgVersion = SaveAsDwgVersion.NewestAvailable
                  };
      }

      return new AcadDatabase(new Database(true, true), options.KeepDatabaseOpen, options.SaveFileName, options.SaveDwgVersion);
    }

    /// <summary>
    /// Provides access to the drawing database of the active document.
    /// </summary>
    /// <returns>The AcadDatabase instance.</returns>
    public static AcadDatabase Active()
    {
      Require.ObjectNotNull(Application.DocumentManager.MdiActiveDocument, "No active document");

      return new AcadDatabase(Application.DocumentManager.MdiActiveDocument.Database, true);
    }

    /// <summary>
    /// Provides access to the drawing database of the active document.
    /// This is an advanced feature, use with caution.
    /// </summary>
    /// <param name="transaction">The transaction to use.</param>
    /// <param name="commitTransaction">True, if the transaction in use should be committed when this instance is disposed of.</param>
    /// <param name="disposeTransaction">True, if the transaction in use should be disposed of when this instance is disposed of.</param>
    /// <returns>The AcadDatabase instance.</returns>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public static AcadDatabase Active(Transaction transaction, bool commitTransaction, bool disposeTransaction)
    {
      Require.ObjectNotNull(Application.DocumentManager.MdiActiveDocument, "No active document");
      Require.ParameterNotNull(transaction, nameof(transaction));
      Require.NotDisposed(transaction.IsDisposed, nameof(Transaction), nameof(transaction));

      return new AcadDatabase(Application.DocumentManager.MdiActiveDocument.Database, transaction, commitTransaction, disposeTransaction);
    }

    /// <summary>
    /// Provides access to the given drawing database.
    /// </summary>
    /// <param name="database">The drawing database to use.</param>
    /// <returns>The AcadDatabase instance.</returns>
    public static AcadDatabase Use(Database database)
    {
      Require.ParameterNotNull(database, nameof(database));
      Require.NotDisposed(database.IsDisposed, nameof(Database), nameof(database));

      return new AcadDatabase(database, true);
    }

    /// <summary>
    /// Provides access to the given drawing database.
    /// This is an advanced feature, use with caution.
    /// </summary>
    /// <param name="database">The drawing database to use.</param>
    /// <param name="transaction">The transaction to use.</param>
    /// <param name="commitTransaction">True, if the transaction in use should be committed when this instance is disposed of.</param>
    /// <param name="disposeTransaction">True, if the transaction in use should be disposed of when this instance is disposed of.</param>
    /// <returns>The AcadDatabase instance.</returns>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public static AcadDatabase Use(Database database, Transaction transaction, bool commitTransaction, bool disposeTransaction)
    {
      Require.ParameterNotNull(database, nameof(database));
      Require.NotDisposed(database.IsDisposed, nameof(Database), nameof(database));
      Require.ParameterNotNull(transaction, nameof(transaction));
      Require.NotDisposed(transaction.IsDisposed, nameof(transaction));

      return new AcadDatabase(database, transaction, commitTransaction, disposeTransaction);
    }

    /// <summary>
    /// Provides read-only access to the drawing database in the given file.
    /// </summary>
    /// <param name="fileName">The name of the drawing database to open.</param>
    /// <param name="options">Options for opening the database.</param>
    /// <returns>The AcadDatabase instance.</returns>
    public static AcadDatabase OpenReadOnly(string fileName, OpenReadOnlyOptions options = null)
    {
      Require.StringNotEmpty(fileName, nameof(fileName));
      Require.FileExists(fileName, nameof(fileName));

      options = options ?? new OpenReadOnlyOptions();

      Database database = GetDatabase(fileName, true, options.Password);
      return new AcadDatabase(database, options.KeepDatabaseOpen);
    }

    /// <summary>
    /// Provides read/write access to the drawing database in the given file.
    /// </summary>
    /// <param name="fileName">The name of the drawing database to open.</param>
    /// <param name="options">Options for opening and closing the database.</param>
    /// <returns>The AcadDatabase instance.</returns>
    public static AcadDatabase OpenForEdit(string fileName, OpenForEditOptions options = null)
    {
      Require.StringNotEmpty(fileName, nameof(fileName));
      Require.FileExists(fileName, nameof(fileName));

      if (options == null)
      {
        options = new OpenForEditOptions()
                  {
                    SaveAsFileName = fileName,
                    KeepDatabaseOpen = false,
                    DwgVersion = SaveAsDwgVersion.DontChange,
                  };
      }

      Database database = GetDatabase(fileName, false, options.Password);
      var outFileName = options.SaveAsFileName ?? fileName;
      return new AcadDatabase(database, options.KeepDatabaseOpen, outFileName, options.DwgVersion);
    }

    /// <summary>
    /// Provides access to the drawing database in the given file.
    /// </summary>
    /// <param name="fileName">The name of the drawing database to open.</param>
    /// <param name="readOnly">If true open the drawing in read-only mode. If false, open it in read/write mode.</param>
    /// <param name="password">The password for the drawing database.</param>
    /// <returns>The Autocad Database instance.</returns>
    private static Database GetDatabase(string fileName, bool readOnly, string password)
    {
      var database = new Database(false, true);
      database.ReadDwgFile(fileName, readOnly ? FileOpenMode.OpenForReadAndReadShare : FileOpenMode.OpenForReadAndWriteNoShare, false, password);
      database.CloseInput(true);
      return database;
    }

    #endregion

    #region Overrides to remove methods from IntelliSense

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override bool Equals(object obj)
      => base.Equals(obj);

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int GetHashCode()
      => base.GetHashCode();

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override string ToString()
      => base.ToString();

    #endregion
  }
}
