using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

class Program
{
  static void Main()
  {
    // 1. Initialize the Shell environment
    var shell = new Shell();
    shell.Run();
  }
}
