﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq2Acad.Tests
{
  public class AcadAssertFailedException : Exception
  {
    public AcadAssertFailedException(string message)
      : base(message)
    {
    }
  }
}
