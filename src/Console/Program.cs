using NewCleanArch.Factories;

static class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Arguments passed to the program.</param>
    public static void Main(string[] args)
    {
        try
        {
            // Add help function
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                Console.WriteLine("======================================================================");
                Console.WriteLine("Clean Architecture Project Creator");
                Console.WriteLine("======================================================================");
                Console.WriteLine();
                Console.WriteLine("Creates a project with clean architecture or add entities and usecases to a clean project.");
                Console.WriteLine();
                Console.WriteLine("Project Arguments:");
                Console.WriteLine("Usage: -np <PATH> <PROJECT_NAME> [-es] [-ui <UI_TYPE>]");
                Console.WriteLine("  -np                Create a new project.");
                Console.WriteLine("  <PATH>             Path where the project will be created.");
                Console.WriteLine("  <PROJECT_NAME>     Project name.");
                Console.WriteLine("  -es                Add external services project.");
                Console.WriteLine("  -ui                Add UI project. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                Console.WriteLine();
                // Console.WriteLine("Entities Arguments:");
                // Console.WriteLine("Usage: -ne <PATH> <ENTITY_NAME1> <ENTITY_NAME2> ...");
                // Console.WriteLine("  -ne                Create a new entity.");
                // Console.WriteLine("  <PATH>             Path to the project.");
                // Console.WriteLine("  <ENTITY_NAMES>      Entity name.");
                return;
            }

            // Execute the command
            switch (args[0].ToLower())
            {
                case "-np":
                    var service = ProjectServiceFactory.Execute(args);
                    service.Execute();
                    break;
                // case "-ne":
                //     break;
                default:
                    throw new Exception("Invalid command.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(-1);
        }
    }
}
