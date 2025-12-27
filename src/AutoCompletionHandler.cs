using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Commands.Helpers {
  public class AutoCompletionHandler : IAutoCompleteHandler {
    public char[] Separators { get; set; } = "abcdefghijklmnopqrstuvwxyz".ToArray();

    private string[] _commands = ["echo", "exit"];

    public AutoCompletionHandler(string[] commands)
    {
      _commands = commands;
    }

    public string[] GetSuggestions(string text, int index) {
      var matches =  _commands.Where(c => c.StartsWith(text))
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
