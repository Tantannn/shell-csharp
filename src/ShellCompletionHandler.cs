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
    // "text" is now the whole line (e.g. "echo myfi")
    if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();

    var parts = text.Split(' ');

    // Logic: If there are no spaces, we are typing the Command.
    // If there is at least one space, we are typing Arguments (Files).
    bool isFirstWord = !text.Contains(' ');

    if (isFirstWord)
    {
      // --- COMMAND COMPLETION ---
      return _commands
        .Where(c => c.StartsWith(text, StringComparison.OrdinalIgnoreCase))
        .ToArray();
    }
    else
    {
      // --- FILE COMPLETION ---
      // "echo myfi" -> we want to complete "myfi"
      string lastWord = parts.Last();

      var fileMatches = GetFileSuggestions(lastWord);

      // 2. IMPORTANT: Reconstruct the line.
      // Since we told ReadLine NOT to split (Separators is empty), 
      // it will replace the ENTIRE line with whatever we return.
      // We must stick the first part of the command back onto the suggestion.

      // Example:
      // Input: "echo myfi"
      // Match: "myfile.txt"
      // We must return: "echo myfile.txt"

      string prefix = text.Substring(0, text.Length - lastWord.Length);

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