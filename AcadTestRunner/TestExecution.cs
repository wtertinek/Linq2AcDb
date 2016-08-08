﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;

namespace AcadTestRunner
{
  public class TestExecution
  {
    [CommandMethod("LoadAndExecuteTest")]
    public static void LoadAndExecuteTest()
    {
      var loaderNotifier = new Notification("TestLoader");
      Notification testNotifier = null;

      try
      {
        var editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        var assemblyPath = editor.GetString(Notification.GetMessage("TestLoader", "Assembly path")).StringResult;
        var testClassName = editor.GetString(Notification.GetMessage("TestLoader", "Class name")).StringResult;
        var testName = editor.GetString(Notification.GetMessage("TestLoader", "AcadTest name")).StringResult;
        testNotifier = new Notification(testName);

        var type = Assembly.LoadFrom(assemblyPath)
                           .GetTypes()
                           .FirstOrDefault(t => t.Name == testClassName);

        if (type == null)
        {
          testNotifier.TestFailed("Class " + testClassName + " not found");
          loaderNotifier.WriteMessage("Test execution finished with errors");
        }
        else
        {
          loaderNotifier.WriteMessage("Class " + type.Name + " loaded");

          var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                           .Select(m => new { Method = m, Attributes = m.GetCustomAttributes(typeof(AcadTestAttribute)).ToArray() })
                           .FirstOrDefault(m => m.Attributes.Any(a => ((AcadTestAttribute)a).TestMethodName == testName));

          if (method == null)
          {
            testNotifier.TestFailed("No AcadTest \"" + testName + "\" found");
            loaderNotifier.WriteMessage("Test execution finished with errors");
          }
          else
          {
            loaderNotifier.WriteMessage("AcadTest " + testName + " found");

            var hasDefaultConstructor = type.GetConstructors()
                                            .Any(c => c.IsPublic &&
                                                       c.GetParameters().Count() == 0);

            if (!hasDefaultConstructor)
            {
              testNotifier.TestFailed("No public default constructor found in class " + testClassName);
              loaderNotifier.WriteMessage("Test execution finished with errors");
            }
            else
            {
              var instance = Activator.CreateInstance(type);
              loaderNotifier.WriteMessage("Instance of " + type.Name + " created");

              loaderNotifier.WriteMessage("Executing AcadTest " + testName + ", invoking method " + method.Method);
              type.InvokeMember(method.Method.Name, BindingFlags.InvokeMethod, null, instance, new object[0]);

              testNotifier.TestPassed();

              loaderNotifier.WriteMessage("Test execution finished");
            }
          }
        }
      }
      catch (AcadAssertFailedException e)
      {
        if (testNotifier != null)
        {
          testNotifier.TestFailed(e.Message, e.StackTrace);
        }

        loaderNotifier.WriteMessage("Test execution finished with errors");
      }
      catch (System.Exception e)
      {
        if (testNotifier != null)
        {
          testNotifier.TestFailed(e);
        }

        loaderNotifier.WriteMessage("Test execution finished with errors");
      }
    }
  }
}
