using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Commands.Helpers {
  public class AutoCompletionHandler : IAutoCompleteHandler {
    public char[] Separators { get; set; } = "abcdefghijklmnopqrstuvwxyz".ToArray();

    private readonly string[] _commands;
    public int _tabCount = 0;
    public string _previousSearch = "";

    public AutoCompletionHandler(string[] commands)
    {
      _commands = commands;
    }

    public string[] GetSuggestions(string text, int index)
    {
      var foundCmd = _commands.Where(c => c.StartsWith(text)).OrderBy(c => c);
      var enumerable = foundCmd.ToArray();
      var matches = enumerable.Select(c => c.Substring(text.Length) + " ")
          .ToArray();
      _previousSearch = text;

      if (matches.Length != 1)
      {
        if (_tabCount == 0)
        {
          _tabCount++;
          Console.Write('\a');
          return [];
        }
        Console.WriteLine();
        Console.WriteLine(string.Join("  ", enumerable));
        Console.Write("$ " + text);
        return [];
      }
      _tabCount = 0;
      return matches;
    }
  }
}
