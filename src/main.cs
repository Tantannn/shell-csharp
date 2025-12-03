class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");

            // Wait for user input
            var command = Console.ReadLine();
            if (String.IsNullOrEmpty(command) || command == "exit") {
                break;
            };
            var arg = command.Split(" ");
            if (arg[0] == "echo")  {
                Console.WriteLine($"{command.Substring(5)}");
            } else {
                Console.WriteLine($"{command}: command not found");
            }
        }
    }
}
