using Kallimakhos.Application.Ports;
using Kallimakhos.CLI.Interfaces;

namespace Kallimakhos.CLI.Services
{
    public class GetArgsFromCli : IGetArgsFromCli
    {
        public SettingsInput Execute(string[] args)
        {
            #region Check project path
            // Get where the project will be saved
            string path = args[1];

            // Check if the path is valid
            if (!Directory.Exists(path))
            {
                try
                {
                    // Try to create the path
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    throw new Exception("Invalid path.");
                }
            }
            #endregion Check project path

            #region Check project name
            // Get the project name
            string projectName = args[2];

            // Check if the project name is valid
            if (projectName.Length == 0)
            {
                throw new Exception("Invalid project name.");
            }
            #endregion Check project name

            #region Define project full path
            // Create a path for the project
            string projectPath = Path.Combine(path, projectName);
            #endregion Define project full path

            #region External Services
            // Check if the user wants External Services (-es)
            bool hasExternalServices = false;
            int index = Array.IndexOf(args, "-es");
            if (index > 0)
            {
                hasExternalServices = true;
            }

            // Check if the user wants External Services and what type is specified
            string[]? nameServices = null;
            if (index > 0 && args.Length > index + 1)
            {
                // Get the services
                nameServices = args[index + 1].Split(',');

                // Check if the services are valid
                if (nameServices.Length == 0)
                {
                    throw new Exception("Invalid services.");
                }
            }
            #endregion External Services

            #region User Interface
            // Check if the user wants UI (-ui) and what type is specified
            string? typeUI = null;
            index = Array.IndexOf(args, "-ui");
            if (index > 0 && args.Length > index + 1)
            {
                typeUI = args[index + 1].ToLower() switch
                {
                    "grpc" => "grpc",
                    "webapi" => "webapi",
                    "webapp" => "webapp",
                    "mvc" => "mvc",
                    "console" => "console",
                    "angular" => "angular",
                    "react" => "react",
                    _ => throw new Exception("Invalid UI type. Valid types: grpc, webapi, webapp, mvc, console, angular, react"),
                };

                // Check if the UI type is valid
                if (typeUI == null)
                {
                    throw new Exception("Invalid UI type. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                }
            }
            #endregion User Interface

            #region Check if the user wants to create entities and what entities are specified
            // Check if the user wants to create entities (-entity)
            bool hasEntities = false;
            index = Array.IndexOf(args, "-entity");
            if (index > 0)
            {
                hasEntities = true;
            }

            // Check if the user wants to create entities and what entities are specified
            string[]? entities = null;
            if (index > 0 && args.Length > index + 1)
            {
                // Get the entities
                entities = args[index + 1].Split(',');

                // Check if the entities are valid
                if (entities.Length == 0)
                {
                    throw new Exception("Invalid entities.");
                }
            }
            #endregion Check if the user wants to create entities and what entities are specified

            #region Check if the user wants CRUD and what entities are specified
            // Check if the user wants CRUD (-crud)
            bool hasCRUD = false;
            index = Array.IndexOf(args, "-crud");
            if (index > 0)
            {
                hasCRUD = true;
            }

            // Check if the user wants CRUD and what entities are specified
            string[]? crudEntities = null;
            if (index > 0 && args.Length > index + 1)
            {
                string tmp = args[index + 1];
                if (tmp == "all")
                {
                    // Get all entities
                    crudEntities = entities;
                }
                else if (!tmp.Contains("-"))
                {
                    // Get the entities
                    crudEntities = args[index + 1].Split(',');

                    // Check if the entities are valid
                    if (crudEntities.Length == 0)
                    {
                        throw new Exception("Invalid entities to generate CRUDs.");
                    }
                }
                else
                {
                    throw new Exception("Invalid entities to generate CRUDs.");
                }
            }
            #endregion Check if the user wants CRUD and what entities are specified

            #region Check if the user wants to use a database and what type is specified
            // Check if the user wants to use a database (-db)
            bool hasDatabase = false;
            index = Array.IndexOf(args, "-db");
            if (index > 0)
            {
                hasDatabase = true;
            }

            // Check if the user wants to use a database and what type is specified
            string? databaseType = null;
            if (index > 0 && args.Length > index + 1)
            {
                string tmp = args[index + 1];
                if (!tmp.Contains("-") && tmp != "none")
                {
                    // Get the database type
                    databaseType = args[index + 1].ToLower() switch
                    {
                        "sqlserver" => "SQL Server",
                        "mysql" => "MySQL",
                        "postgresql" => "PostgreSQL",
                        "mongodb" => "MongoDB",
                        _ => throw new Exception("Invalid database type. Valid types: sqlserver, mysql, postgresql, mongodb"),
                    };
                }
            }
            #endregion Check if the user wants to use a database and what type is specified

            #region Check if the user wants to use a repository
            // Check if the user wants to use a repository (-repo)
            bool hasRepository = false;
            index = Array.IndexOf(args, "-repo");
            if (index > 0)
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