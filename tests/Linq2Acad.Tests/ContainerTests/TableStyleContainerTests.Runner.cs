﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Linq2Acad.Tests
{
  [TestClass]
  [DebuggerStepThrough]
  public class TableStyleContainerTests_
  {
    [TestMethod]
    [TestCategory("Container Tests")]
    public void TestCreateTableStyle()
    {
      var result = AcadTestRunner.TestRunner.RunTest(typeof(TableStyleContainerTests), "TestCreateTableStyle");

      if (!result.Passed)
      {
        result.DebugPrintFullOutput("TestCreateTableStyle");
        Assert.Fail(result.Message);
      }
    }
  }
}
