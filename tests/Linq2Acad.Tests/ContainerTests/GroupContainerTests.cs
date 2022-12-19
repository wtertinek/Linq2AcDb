﻿using System;
using System.Linq;
using Linq2Acad;
using Autodesk.AutoCAD.DatabaseServices;
using AcadTestRunner;

namespace Linq2Acad.Tests
{
  public class GroupContainerTests
  {
    [AcadTest]
    public void TestCreateGroup()
    {
      var newId = ObjectId.Null;

      using (var db = AcadDatabase.Active())
      {
        var newGroup = db.Groups.Create("NewGroup");
        newId = newGroup.ObjectId;
      }

      AcadAssert.That.GroupDictionary.Contains("NewGroup");
      AcadAssert.That.GroupDictionary.Contains(newId);
    }
  }
}
