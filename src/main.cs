using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    // 1. Define a dictionary to map command names (strings) to Actions
    static readonly Dictionary<string, Action<string[]>> commands = new();

    static void Main()
    {
        // 2. Register your commands here
        RegisterCommands();

        while (true)
        {
            Console.Write("$ ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;

            // 3. Smart Parsing: Split by space but keep the rest of the arguments
            var parts = input.Trim().Split(' ');
            var commandName = parts[0];
            var args = parts.Skip(1).ToArray(); // Everything after the command name

            // 4. Handle "exit" separately to break the loop cleanly
            if (commandName == "exit") break;

            // 5. Execute command if it exists, otherwise print error
            if (commands.TryGetValue(commandName, out var command))
            {
                command(args);
            }
            else
            {
                Console.WriteLine($"{commandName}: command not found");
            }
        }
    }

    static void RegisterCommands()
    {
        commands.Add("echo", (args) =>
        {
            // Join arguments back together to print them
            Console.WriteLine(string.Join(" ", args));
        });

        commands.Add("type", (args) =>
        {
            if (args.Length == 0) return;

            var targetCmd = args[0];

            // Check if the command is in our dictionary or is a special keyword
            if (commands.ContainsKey(targetCmd) || targetCmd == "exit")
            {
                Console.WriteLine($"{targetCmd} is a shell builtin");
            }
            else
            {
                bool isFound = false;

                try
                {
                    // Use ProcessStartInfo to find the executable (most reliable)
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = targetCmd,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    // Try to find the full path
                    var fullPath = Path.Combine(Environment.SystemDirectory, targetCmd);

                    // Check common extensions on Windows
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        string[] winExtensions = [".exe", ".cmd", ".bat", ".com", ""];
                        foreach (var ext in winExtensions)
                        {
                            var testPath = Path.Combine(Environment.SystemDirectory, targetCmd + ext);
                            if (File.Exists(testPath))
                            {
                                isFound = true;
                                Console.WriteLine($"{targetCmd} is {testPath}");
                                break;
                            }
                        }
                    }
                    else // Unix/Linux/Mac
                    {
                        // Use which command simulation
                        var paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? Array.Empty<string>();
                        foreach (var path in paths)
                        {
                            if (string.IsNullOrEmpty(path)) continue;
                            var testPath = Path.Combine(path, targetCmd);
                            if (File.Exists(testPath) && (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                                                          RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
                            {
                                isFound = true;
                                Console.WriteLine($"{targetCmd} is {testPath}");
                                break;
                            }
                        }
                    }

                    if (!isFound)
                    {
                        Console.WriteLine($"{targetCmd}: not found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error finding {targetCmd}: {ex.Message}");
                }
            }
        });

        // Example: easy to add a 'clear' command now
        commands.Add("clear", (_) => Console.Clear());
    }
}