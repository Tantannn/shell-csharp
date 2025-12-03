string[] BUILTIN_CMDS = ["echo", "type", "exit"];

while (true)
{
    Console.Write("$ ");
    // Wait for user input
    var inputStr = Console.ReadLine();

    if (inputStr == "exit 0")
    {
        Environment.Exit(0);
    }

    var parts = inputStr.Split();
    var cmd = parts[0];
    var inputArgs = parts[1..];

    switch (cmd)
    {
        case "echo":
            Console.WriteLine(String.Join(" ", inputArgs));
            break;
        case "type":
            var inputArg = inputArgs[0];

            var isBuiltinCmd = BUILTIN_CMDS.Contains(inputArg);
            if (isBuiltinCmd)
            {
                Console.WriteLine($"{inputArg} is a shell builtin");
            }
            else
            {
                var pathsStr = Environment.GetEnvironmentVariable("PATH");
                var pathsArr = pathsStr.Split(":");
                bool isFound = false;
                foreach (var path in pathsArr)
                {
                    var joinedPath = Path.Join(path, inputArg);
                    if (File.Exists(joinedPath))
                    {
                        isFound = true;
                        Console.WriteLine($"{inputArg} is {joinedPath}");
                        break;
                    }
                }

                if (!isFound)
                {
                    Console.WriteLine($"{inputArg}: not found");
                }
            }

            break;
        default:
            Console.WriteLine($"{inputStr}: command not found");
            break;
    }
}