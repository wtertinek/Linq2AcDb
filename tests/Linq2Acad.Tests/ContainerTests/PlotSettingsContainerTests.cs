﻿using System;
using System.Linq;
using Linq2Acad;
using Autodesk.AutoCAD.DatabaseServices;
using AcadTestRunner;

namespace Linq2Acad.Tests
{
  public class PlotSettingsContainerTests
  {
    [AcadTest]
    public void TestCreatePlotSettings()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newPlotSettings = db.PlotSettings.Create("NewPlotSettings", true);
        newId = newPlotSettings.ObjectId;
      }

      AcadAssert.That.PlotSettingsDictionary.Contains("NewPlotSettings");
      AcadAssert.That.PlotSettingsDictionary.Contains(newId);
    }
  }
}
