using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ShellAutoCompleter : IAutoCompleteHandler
{
  private readonly List<string> _commands;

  public ShellAutoCompleter(IEnumerable<string> commands)
  {
    _commands = commands.ToList();
  }

  // 1. IMPORTANT: Set Separators to empty.
  // This forces the library to send the FULL line to GetSuggestions.
  // If you set this to ' ', the library splits the text for you, breaking context.
  public char[] Separators { get; set; } = new char[0];

  public string[] GetSuggestions(string text, int index)
  {
    if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();

    var parts = text.Split(' ');
    bool isFirstWord = !text.Contains(' ');

    if (isFirstWord)
    {
      // 1. Get ALL matches first
      var matches = _commands
        .Where(c => c.StartsWith(text, StringComparison.OrdinalIgnoreCase))
        .ToArray();

      // 2. Logic: Add space ONLY if it's a unique match
      if (matches.Length == 1)
      {
        return new[] { matches[0] + " " };
      }

      if (matches.Length > 1)
      {
        Console.WriteLine();
        foreach (var m in matches)
        {
          Console.WriteLine(m + "\t");
        }
      }

      var x = parts[0];

      return ["$" + x];
    }
    else
    {
      // --- FILE COMPLETION ---
      string lastWord = parts.Last();
      var fileMatches = GetFileSuggestions(lastWord);
      string prefix = text.Substring(0, text.Length - lastWord.Length);

      // Same logic for files: only add space if it's a unique file match
      // (Optional: usually shells add space for files too, unless it's a directory)
      if (fileMatches.Length == 1)
      {
        return new[] { prefix + fileMatches[0] + " " };
      }

      return fileMatches
        .Select(match => prefix + match)
        .ToArray();
    }
  }

  private string[] GetFileSuggestions(string partialPath)
  {
    try
    {
      string directory = ".";
      string searchPattern = partialPath;

      // Handle subdirectories (e.g. "Folder\fil")
      if (partialPath.Contains(Path.DirectorySeparatorChar) || partialPath.Contains(Path.AltDirectorySeparatorChar))
      {
        directory = Path.GetDirectoryName(partialPath);
        searchPattern = Path.GetFileName(partialPath);

        if (string.IsNullOrEmpty(directory)) directory = ".";
      }

      // Find matches
      var matches = Directory.GetFileSystemEntries(directory, searchPattern + "*")
        .Select(path => Path.GetFileName(path)) // Get just the file name
        .ToArray();

      return matches;
    }
    catch
    {
      return Array.Empty<string>();
    }
  }
}