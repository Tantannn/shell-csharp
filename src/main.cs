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
            if (command != null) break;
            Console.WriteLine($"{command}: command not found");
        }
    }
}
