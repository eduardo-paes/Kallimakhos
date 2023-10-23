using Kallimakhos.Application.Ports;
using Kallimakhos.CLI.Interfaces;

namespace Kallimakhos.CLI.Services
{
    public class GetArgsInteractively : IGetArgsInteractively
    {
        public SettingsInput Execute()
        {
            Console.WriteLine("Clean Architecture Project Creator");
            Console.WriteLine("===================================");

            #region Check project path
            string path;
            do
            {
                Console.Write("Enter the path where the project will be saved: ");
                path = Console.ReadLine();
            } while (!Directory.Exists(path));
            #endregion Check project path

            #region Check project name
            string projectName;
            do
            {
                Console.Write("Enter the project name: ");
                projectName = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(projectName));
            #endregion Check project name

            #region Define project full path
            string projectPath = Path.Combine(path, projectName);
            #endregion Define project full path

            #region External Services
            bool hasExternalServices = false;
            string[]? nameServices = null;
            Console.Write("Do you want to add external services project? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                hasExternalServices = true;
                Console.Write("Enter a comma-separated list of service names (e.g., Service1,Service2): ");
                string servicesInput = Console.ReadLine();
                nameServices = string.IsNullOrWhiteSpace(servicesInput) ? null : servicesInput.Split(',');
            }
            #endregion External Services

            #region User Interface
            string? typeUI = null;
            Console.Write("Do you want to add a UI project? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                Console.Write("Enter the UI type (valid types: grpc, webapi, webapp, mvc, console, angular, react): ");
                string uiType = Console.ReadLine()?.ToLower();
                if (!string.IsNullOrWhiteSpace(uiType) && (uiType == "grpc" || uiType == "webapi" || uiType == "webapp" ||
                    uiType == "mvc" || uiType == "console" || uiType == "angular" || uiType == "react"))
                {
                    typeUI = uiType;
                }
                else
                {
                    Console.WriteLine("Invalid UI type. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                    throw new Exception("Invalid user input.");
                }
            }
            #endregion User Interface

            #region Check if the user wants to create entities and what entities are specified
            bool hasEntities = false;
            string[]? entities = null;
            Console.Write("Do you want to create entities? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                hasEntities = true;
                Console.Write("Enter a comma-separated list of entity names (e.g., Entity1,Entity2): ");
                string entitiesInput = Console.ReadLine();
                entities = string.IsNullOrWhiteSpace(entitiesInput) ? null : entitiesInput.Split(',');
            }
            #endregion Check if the user wants to create entities and what entities are specified

            #region Check if the user wants CRUD and what entities are specified
            bool hasCRUD = false;
            string[]? crudEntities = null;
            Console.Write("Do you want to generate CRUD use cases? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                hasCRUD = true;
                Console.Write("Enter a comma-separated list of entity names for CRUD (e.g., Entity1,Entity2) or 'all' for all entities: ");
                string crudInput = Console.ReadLine();
                if (crudInput == "all")
                {
                    crudEntities = entities;
                }
                else if (!string.IsNullOrWhiteSpace(crudInput))
                {
                    crudEntities = crudInput.Split(',');
                }
            }
            #endregion Check if the user wants CRUD and what entities are specified

            #region Check if the user wants to use a database and what type is specified
            bool hasDatabase = false;
            string? databaseType = null;
            Console.Write("Do you want to use a database? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                hasDatabase = true;
                Console.Write("Enter the database type (valid types: sqlserver, mysql, postgresql, mongodb) or 'none': ");
                string dbType = Console.ReadLine()?.ToLower();
                if (!string.IsNullOrWhiteSpace(dbType) && (dbType == "sqlserver" || dbType == "mysql" || dbType == "postgresql" || dbType == "mongodb" || dbType == "none"))
                {
                    databaseType = dbType == "none" ? null : dbType;
                }
                else
                {
                    Console.WriteLine("Invalid database type. Valid types: sqlserver, mysql, postgresql, mongodb");
                    throw new Exception("Invalid user input.");
                }
            }
            #endregion Check if the user wants to use a database and what type is specified

            #region Check if the user wants to use a repository
            bool hasRepository = false;
            Console.Write("Do you want to use a repository? (yes/no): ");
            if (Console.ReadLine()?.ToLower()[0] == 'y')
            {
                hasRepository = true;
            }
            #endregion Check if the user wants to use a repository

            // Create the settings
            return new SettingsInput
            {
                // Project
                ProjectName = projectName,
                ProjectPath = projectPath,

                // External Services
                HasExternalServices = hasExternalServices,
                ServiceNames = nameServices,

                // User Interface
                TypeUI = typeUI,
                NameUI = typeUI,

                // CRUDs
                HasCRUDs = hasCRUD,
                CRUDEntities = crudEntities,

                // Database
                HasDatabase = hasDatabase,
                DatabaseType = databaseType,
                HasRepositories = hasRepository,

                // Entities
                HasEntities = hasEntities,
                EntityNames = entities
            };
        }
    }
}