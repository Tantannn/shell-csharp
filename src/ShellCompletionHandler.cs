using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Commands.Helpers {
  public class AutoCompletionHandler : IAutoCompleteHandler {
    public char[] Separators { get; set; }

    private string[] commands = ["echo", "exit"];

    public AutoCompletionHandler() {
      Separators = "abcdefghijklmnopqrstuvwxyz".ToArray();
    }

    public string[] GetSuggestions(string text, int index) {
      return commands.Where(c => c.StartsWith(text))
          .Select(c => c.Substring(text.Length) + " ")
          .ToArray();
    }
  }
}
