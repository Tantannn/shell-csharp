using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Commands.Helpers
{
  public class AutoCompletionHandler : IAutoCompleteHandler
  {
    public char[] Separators { get; set; } = "abcdefghijklmnopqrstuvwxyz".ToArray();

    private readonly string[] _commands;
    public int _tabCount = 0;

    public AutoCompletionHandler(string[] commands)
    {
      _commands = commands;
    }
    private string GetSharedPrefix(string[] matches)
    {
        if (matches.Length == 0) return string.Empty;
        
        string prefix = matches[0];
        for (int i = 1; i < matches.Length; i++)
        {
            int j = 0;
            while (j < prefix.Length && j < matches[i].Length && prefix[j] == matches[i][j])
            {
                j++;
            }
            prefix = prefix.Substring(0, j);
            if (prefix == string.Empty) break;
        }
        return prefix;
    }

    public string[] GetSuggestions(string text, int index)
    {
        // 1. Get matches
        var enumerable = _commands.Where(c => c.StartsWith(text)).OrderBy(c => c).ToArray();

        // No matches -> Ring bell
        if (enumerable.Length == 0)
        {
            Console.Write('\a');
            return [];
        }

        // 2. Exactly one match -> Return it with a space
        if (enumerable.Length == 1)
        {
            _tabCount = 0;
            return [enumerable[0].Substring(text.Length) + " "];
        }

        // 3. Multiple matches
        if (_tabCount == 0)
        {
            // Find the shared part among all matches
            // Start with the first match (without a space)
            var firstMatch = enumerable[0].Substring(text.Length);
            var commonSubString = firstMatch;

            foreach (var fullCmd in enumerable.Skip(1))
            {
                var matchPart = fullCmd.Substring(text.Length);
                var i = 0;
                while (i < commonSubString.Length && i < matchPart.Length && commonSubString[i] == matchPart[i])
                {
                    i++;
                }
                commonSubString = commonSubString.Substring(0, i);
            }

            // If there's a common prefix we can fill in, return it to the library!
            if (commonSubString.Length > 0)
            {
                _tabCount = 0; 
                return [commonSubString]; // The library will handle the printing
            }

            // Nothing common to fill? Just ring the bell
            _tabCount++;
            Console.Write('\a');
            return [];
        }

        // 4. Second Tab -> Show all options
        Console.WriteLine();
        Console.WriteLine(string.Join("  ", enumerable)); // Use 2 spaces per requirements
        Console.Write("$ " + text);
        _tabCount = 0;
        return [];
    }
  }
}