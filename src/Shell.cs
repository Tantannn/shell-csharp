using System.Diagnostics;
using System.Text;

public class Shell

{
  private readonly Dictionary<string, Action<string[]>> _builtIns;
  private readonly PathResolver _pathResolver;
  private bool _isRunning = true;

  public Shell()
  {
    _pathResolver = new PathResolver();

    // Register commands using a Dictionary (The Command Pattern)
    _builtIns = new Dictionary<string, Action<string[]>>
    {
      { "exit", HandleExit },
      { "echo", HandleEcho },
      { "type", HandleType }
    };
  }

  public void Run()
  {
    while (_isRunning)
    {
      Console.Write("$ ");

      // FIX: ReadLine must be INSIDE the loop
      var input = Console.ReadLine();

      // Handle Ctrl+C or empty input
      if (input == null) break;
      if (string.IsNullOrWhiteSpace(input)) continue;

      var parts = ParseString(input);
      var command = parts[0];
      var args = parts.Skip(1).ToArray();

      Execute(command, args);
    }
  }

  private List<string> ParseString(string input)
  {
    if (input == "") return [];
    var inSingleQuote = false;
    var inDoubleQuote = false;
    var hasTokenStarted = false;
    var isAfterSlash = false;
    var currentToken = new StringBuilder();
    var args = new List<string>();
    foreach (var c in input)
    {
      if (c == '\\')
      {
        isAfterSlash = true;
        continue;
      }

      if (isAfterSlash)
      {
        isAfterSlash = false;
        currentToken.Append(c);
        continue;
      }

      switch (c)
      {
        // case '\\':
        //   isAfterSlash = true;
        //   break;
        case '"':
          inDoubleQuote = !inDoubleQuote;
          hasTokenStarted = true;
          break;
        case '\'' when !inDoubleQuote:
          inSingleQuote = !inSingleQuote;
          hasTokenStarted = true;
          break;
        case ' ' when (!inSingleQuote && !inDoubleQuote):
        {
          if (!hasTokenStarted) continue;
          hasTokenStarted = false;
          args.Add(currentToken.ToString());
          currentToken.Clear();

          break;
        }
        default:
          hasTokenStarted = true;
          currentToken.Append(c);
          break;
      }
    }

    if (hasTokenStarted)
    {
      args.Add(currentToken.ToString());
    }

    return args;
  }

  private void Execute(string command, string[] args)
  {
    if (_builtIns.ContainsKey(command))
    {
      _builtIns[command](args);
      return;
    }

    var executablePath = _pathResolver.FindExecutable(command);
    if (executablePath != null)
    {
      RunExternalProcess(command, args);
    }
    else
    {
      Console.WriteLine($"{command}: command not found");
    }
  }

  private void HandleExit(string[] args)
  {
    int exitCode = 0;
    if (args.Length > 0 && int.TryParse(args[0], out int code))
    {
      exitCode = code;
    }

    Environment.Exit(exitCode);
  }

  private void HandleEcho(string[] args)
  {
    var joinedArgs = string.Join(' ', args);
    Console.WriteLine(joinedArgs);
  }

  private void HandleType(string[] args)
  {
    if (args.Length == 0) return;

    var target = args[0];

    if (_builtIns.ContainsKey(target))
    {
      Console.WriteLine($"{target} is a shell builtin");
    }
    else
    {
      var path = _pathResolver.FindExecutable(target);
      if (path != null)
        Console.WriteLine($"{target} is {path}");
      else
        Console.WriteLine($"{target}: not found");
    }
  }

  private void RunExternalProcess(string path, string[] args)
  {
    try
    {
      var startInfo = new ProcessStartInfo
      {
        FileName = path,
        UseShellExecute = false,
        RedirectStandardOutput = false, // Let the child process write directly to console
        RedirectStandardError = false
      };

      // Add arguments correctly
      foreach (var arg in args) startInfo.ArgumentList.Add(arg);

      using var process = Process.Start(startInfo);
      process?.WaitForExit();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error executing {path}: {ex.Message}");
    }
  }
}