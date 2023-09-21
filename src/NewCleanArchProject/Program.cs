using NewCleanArchProject.Factories;

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
            // Add a help function
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                Console.WriteLine("======================================================================");
                Console.WriteLine("Clean Architecture Project Creator");
                Console.WriteLine("======================================================================");
                Console.WriteLine();
                Console.WriteLine("Creates a project with a clean architecture and adds Entities, UseCases, CRUDs, Services, and Repositories.");
                Console.WriteLine();
                Console.WriteLine("Project Arguments:");
                Console.WriteLine("Usage: -np <PATH> <PROJECT_NAME> [-es <S1,...,Sn>] [-ui <UI_TYPE>] [-entity <E1,...,En>] [-crud <E1,...,En> | all] [-db <DB_TYPE> | none] [-repo]");
                Console.WriteLine("  -np                             Create a new project using flags.");
                Console.WriteLine("  ... <PATH>                      Path where the project will be created.");
                Console.WriteLine("  ... <PROJECT_NAME>              Project name.");
                Console.WriteLine("  ... -es <S1,...,Sn>             Add an external services project.");
                Console.WriteLine("  ... -ui <UI_TYPE>               Add a UI project. Valid types: grpc, webapi, webapp, mvc, console, angular, react.");
                Console.WriteLine("  ... -entity <E1,...,En>         Add entities to the project.");
                Console.WriteLine("  ... -crud <E1,...,En> | all     Add CRUD use cases for the specified entities.");
                Console.WriteLine("  ... -db <DB_TYPE> | none        Add a Data project. Valid DB clients if needed: sqlserver, mysql, postgresql, mongodb.");
                Console.WriteLine("  ... -repo                       Add repositories to the project.");
                Console.WriteLine("  start                           Allow the creation of a new project with detailed configuration through CLI interaction.");
                Console.WriteLine();
                return;
            }

            // Execute the command
            var service = args[0].ToLower() switch
            {
                "-np" => ProjectServiceFactory.Execute(args),
                "start" => ProjectServiceFactory.Execute(),
                _ => throw new Exception("Invalid command."),
            };
            service.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(-1);
        }
    }
}
