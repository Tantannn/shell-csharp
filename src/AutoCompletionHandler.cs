using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Commands.Helpers {
  public class AutoCompletionHandler : IAutoCompleteHandler {
    public char[] Separators { get; set; } = "abcdefghijklmnopqrstuvwxyz".ToArray();

    private string[] commands = ["echo", "exit"];

    public string[] GetSuggestions(string text, int index) {
      var matches =  commands.Where(c => c.StartsWith(text))
          .Select(c => c.Substring(text.Length) + " ")
          .ToArray();
      if (matches.Length == 0)
      {
        Console.Write('\a');
        return [];
      }
      return matches;
    }
  }
}
