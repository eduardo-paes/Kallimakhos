using Kallimakhos.CLI.Interfaces;

namespace Kallimakhos.CLI.Services
{
    public class DisplayHelpMenu : IDisplayHelpMenu
    {
        public void Execute()
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
        }
    }
}