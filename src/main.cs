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
                var pathsStr = Environment.GetEnvironmentVariable("PATH");
                var pathsArr = pathsStr.Split(":");
                var isFound = false;
                foreach (var path in pathsArr)
                {
                    var joinedPath = Path.Join(path, targetCmd);
                    if (!File.Exists(joinedPath)) continue;
                    isFound = true;
                    Console.WriteLine($"{targetCmd} is {joinedPath}");
                    break;
                }

                if (!isFound)
                {
                    Console.WriteLine($"{targetCmd}: not found");
                }
            }
        });

        // Example: easy to add a 'clear' command now
        commands.Add("clear", (_) => Console.Clear());
    }
}