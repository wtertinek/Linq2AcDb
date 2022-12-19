﻿using System;
using System.Linq;
using Linq2Acad;
using Autodesk.AutoCAD.DatabaseServices;
using AcadTestRunner;

namespace Linq2Acad.Tests
{
  public class ViewContainerTests
  {
    [AcadTest]
    public void TestCreateView()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newView = db.Views.Create("NewView");
        newId = newView.ObjectId;
      }

      AcadAssert.That.ViewTable.Contains("NewView");
      AcadAssert.That.ViewTable.Contains(newId);
    }

    [AcadTest]
    public void TestAddView()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newView = new ViewTableRecord() { Name = "NewView" };
        db.Views.Add(newView);
        newId = newView.ObjectId;
      }

      AcadAssert.That.ViewTable.Contains(newId);
    }
  }
}
