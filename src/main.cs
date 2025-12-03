using System.Diagnostics;

public class Program
{
  private static List<string> _builtInCommands =
    new List<string> { "exit", "echo", "type" };

  private static string? BuiltinPath(string command, string[] dirs)
  {
    foreach (var dir in dirs.Where(Directory.Exists))
    {
      var potentialFile = Path.Combine(dir, command);

      if (File.Exists(potentialFile))
      {
        try
        {
          var mode = File.GetUnixFileMode(potentialFile);

          // Check if User, Group, or Other has Execute permission
          bool isExecutable =
            (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute |
                     UnixFileMode.OtherExecute)) != 0;

          if (isExecutable)
          {
            return potentialFile;
          }
        }
        catch (Exception)
        {
          // If we can't read permissions (or on Windows), fallback to just
          // returning existing files
          return potentialFile;
        }
      }
    }

    return null;
  }

  public static int Main()
  {
    string? path = Environment.GetEnvironmentVariable("PATH");
    string[] dirs = new string[0];
    if (path != null)
    {
      char sep = Path.PathSeparator; // ; on Windows, : on Linux/macOS
      dirs = path.Split(sep, StringSplitOptions.RemoveEmptyEntries);
    }

    while (true)
    {
      Console.Write("$ ");
      string command = Console.ReadLine();
      string[] args = command.Split(" ");
      if (args.Length >= 1)
      {
        if (args[0] == "exit")
        {
          if (args.Length == 2 && int.TryParse(args[1], out int code))
          {
            return code;
          }

          return 0;
        }
        else if (args[0] == "echo")
        {
          Console.WriteLine(command.Substring(5));
        }
        else if (args[0] == "type")
        {
          if (args.Length >= 2)
          {
            if (_builtInCommands.Contains(args[1]))
              Console.WriteLine($"{args[1]} is a shell builtin");
            else
            {
              var fullPath = BuiltinPath(args[1], dirs);
              if (fullPath != null)
                Console.WriteLine($"{args[1]} is {fullPath}");
              else
                Console.WriteLine($"{args[1]}: not found");
            }
          }
        }
        else
        {
          var fullPath = BuiltinPath(args[0], dirs);
          if (fullPath != null)
            Process.Start(args[0], args[1..]).WaitForExit();
          else
            Console.WriteLine($"{command}: command not found");
        }
      }
    }
  }
}