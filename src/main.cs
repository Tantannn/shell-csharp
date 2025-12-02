class Program
{
    static void Main()
    {
        while (true)
        {
            // TODO: Uncomment the code below to pass the first stage
            Console.Write("$ ");

            // Wait for user input
            var command = Console.ReadLine();
            if (String.IsNullOrEmpty(command) || command == "exit") {
                break;
            };
            Console.WriteLine($"{command}: command not found");
        }
    }
}
