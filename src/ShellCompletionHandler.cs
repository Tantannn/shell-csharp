using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ShellAutoCompleter : IAutoCompleteHandler
{
    private readonly List<string> _commands;

    // Pass in your built-ins and PATH executables here
    public ShellAutoCompleter(IEnumerable<string> commands)
    {
        _commands = commands.ToList();
    }

    // Characters that separate words. We need this to split the input.
    public char[] Separators { get; set; } = new char[] { ' ' };

    public string[] GetSuggestions(string text, int index)
    {
        // 1. Parse the input to see which "word" we are on
        // 'text' is the full line up to the cursor. 
        // We handle simple splitting here, but you might need more complex parsing for quotes later.
        var parts = text.Split(' ');
        
        string wordToComplete = parts.LastOrDefault() ?? "";
        
        // 2. LOGIC: Are we typing the first word (Command) or later words (Arguments)?
        bool isFirstWord = parts.Length == 1;

        if (isFirstWord)
        {
            // --- COMMAND COMPLETION ---
            return _commands
                .Where(c => c.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
        else
        {
            // --- FILE/DIRECTORY COMPLETION ---
            return GetFileSuggestions(wordToComplete);
        }
    }

    private string[] GetFileSuggestions(string partialPath)
    {
        try
        {
            // Determine the directory to search in
            // If they typed "C:\Use", directory is "C:\" and search pattern is "Use*"
            // If they typed "myfi", directory is "." (current) and search pattern is "myfi*"
            
            string directory = ".";
            string searchPattern = partialPath;
            string prefix = ""; // What we prepend to the result

            if (partialPath.Contains(Path.DirectorySeparatorChar) || partialPath.Contains(Path.AltDirectorySeparatorChar))
            {
                directory = Path.GetDirectoryName(partialPath);
                searchPattern = Path.GetFileName(partialPath);
                prefix = directory + Path.DirectorySeparatorChar;
                
                if (string.IsNullOrEmpty(directory)) directory = "."; // Handle root case
            }

            // Get Files and Directories that match
            var files = Directory.GetFileSystemEntries(directory, searchPattern + "*")
                                 .Select(path => prefix + Path.GetFileName(path)) // Rebuild the full relative path
                                 .ToArray();

            return files;
        }
        catch
        {
            // If the path is invalid (e.g. invalid characters), return nothing
            return Array.Empty<string>();
        }
    }
}
