﻿using System;
using System.Linq;
using Linq2Acad;
using Autodesk.AutoCAD.DatabaseServices;
using AcadTestRunner;

namespace Linq2Acad.Tests
{
  public class DimStyleContainerTests
  {
    [AcadTest]
    public void TestCreateDimStyle()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newDimStyle = db.DimStyles.Create("NewDimStyle");
        newId = newDimStyle.ObjectId;
      }

      AcadAssert.That.DimStyleTable.Contains("NewDimStyle");
      AcadAssert.That.DimStyleTable.Contains(newId);
    }

    [AcadTest]
    public void TestAddDimStyle()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newDimStyle = new DimStyleTableRecord() { Name = "NewDimStyle" };
        db.DimStyles.Add(newDimStyle);
        newId = newDimStyle.ObjectId;
      }

      AcadAssert.That.DimStyleTable.Contains(newId);
    }
  }
}
