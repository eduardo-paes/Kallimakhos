using System.Diagnostics;
using NewCleanArchProject.Models;
using NewCleanArchProject.Interfaces;

namespace NewCleanArchProject.Services
{
    public class CreateProjectService : ICreateProjectService
    {
        private readonly Settings _settings;
        private readonly string _currentPath;

        public CreateProjectService(Settings settings)
        {
            _settings = settings;
            _currentPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public void Execute()
        {
            #region Project Initialization

            // Create a folder for the project
            Directory.CreateDirectory(_settings.ProjectPath);

            // Add readme file
            File.WriteAllText($"{_settings.ProjectPath}/README.md", $"# {_settings.ProjectName}");

            // Create a folder for the project source code
            Directory.CreateDirectory($"{_settings.ProjectPath}/src");

            // Change to the project folder
            Directory.SetCurrentDirectory(_settings.ProjectPath);

            // Execute command to generate a .gitignore file
            ExecuteProcess("dotnet", $"new gitignore");

            // Change to the source folder
            Directory.SetCurrentDirectory($"{_settings.ProjectPath}/src");

            // Generate the project solution
            ExecuteProcess("dotnet", $"new sln --name {_settings.ProjectName}");

            #endregion Project Initialization

            #region Project Creation

            #region Domain
            // Generate domain project
            ExecuteProcess("dotnet", $"new classlib -n {_settings.ProjectName}.Domain");
            ExecuteProcess("dotnet", $"sln {_settings.ProjectName}.sln add {_settings.ProjectName}.Domain");
            File.Delete($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Class1.cs");

            // Create domain default directories
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Entities");
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Validations");

            // Add validation class
            string templateValidation = File.ReadAllText(Path.Combine(_currentPath, "Templates/BaseExceptionValidation.txt"));
            templateValidation = templateValidation
                .Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Validations")
                .Replace("{{Type}}", "Entity");
            File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Validations/EntityException.cs", templateValidation);

            // If there are entities
            if (_settings.HasEntities)
            {
                // If entities were provided
                if (_settings.NameEntities != null)
                {
                    string template = File.ReadAllText(Path.Combine(_currentPath, "Templates/Entity.txt"));
                    template = template.Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Entities");

                    // Create entities with capital letters
                    string entityName, tmp;
                    foreach (var entity in _settings.NameEntities)
                    {
                        // Capitalize the first letter of the entity
                        entityName = entity[..1].ToUpper() + entity[1..];

                        // Create the repository file using the template
                        tmp = template.Replace("{{EntityName}}", entityName);
                        File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Entities/{entityName}.cs", tmp);
                    }
                }
            }

            // If there are repositories
            if (_settings.HasRepositories)
            {
                Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces");
                Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Repositories");

                // Convert the entities to a list of strings
                List<string> entities = new();
                if (_settings.NameEntities != null)
                {
                    entities = _settings.NameEntities.ToList();
                }

                // If there are CRUDs
                if (_settings.HasCRUDs)
                {
                    // Create a folder for the base repository
                    Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Repositories/Base");

                    // Create a base repository with CRUD operations
                    string template = File.ReadAllText(Path.Combine(_currentPath, "Templates/ICrudRepository.txt"));
                    template = template.Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Interfaces.Repositories.Base");
                    File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Repositories/Base/ICrudRepository.cs", template);

                    // If CRUD entities were provided
                    if (_settings.CRUDEntities != null)
                    {
                        // Create repositories with CRUD operations
                        template = File.ReadAllText(Path.Combine(_currentPath, "Templates/ICrudEntityRepository.txt"));
                        template = template
                            .Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Interfaces.Repositories")
                            .Replace("{{ProjectName}}", $"{_settings.ProjectName}");

                        // Create repositories with CRUD operations
                        string entityName, tmp;
                        foreach (var entity in _settings.CRUDEntities)
                        {
                            // Remove the entity from the list
                            entities.Remove(entity);

                            // Capitalize the first letter of the entity
                            entityName = entity[..1].ToUpper() + entity[1..];

                            // Create the repository file using the template
                            tmp = template.Replace("{{EntityName}}", entityName);
                            File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Repositories/I{entityName}Repository.cs", tmp);
                        }
                    }
                }

                // If entities were provided
                if (entities.Count > 0)
                {
                    // Create repositories with CRUD operations
                    string template = File.ReadAllText(Path.Combine(_currentPath, "Templates/IEntityRepository.txt"));
                    template = template.Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Interfaces.Repositories");

                    // Create repositories
                    string entityName, tmp;
                    foreach (var entity in entities)
                    {
                        // Capitalize the first letter of the entity
                        entityName = entity[..1].ToUpper() + entity[1..];

                        // Create the repository file using the template
                        tmp = template.Replace("{{EntityName}}", entityName);
                        File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Repositories/I{entityName}Repository.cs", tmp);
                    }
                }
            }

            // If there are services
            if (_settings.HasExternalServices)
            {
                Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces");
                Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Services");

                // If services were provided
                if (_settings.NameServices != null)
                {
                    // Create services
                    string template = File.ReadAllText(Path.Combine(_currentPath, "Templates/IService.txt"));
                    template = template.Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Domain.Interfaces.Services");

                    // Create services
                    string serviceName, tmp;
                    foreach (var service in _settings.NameServices)
                    {
                        // Capitalize the first letter of the service
                        serviceName = service[..1].ToUpper() + service[1..];

                        // Create the repository file using the template
                        tmp = template.Replace("{{ServiceName}}", serviceName);
                        File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/Interfaces/Services/I{serviceName}Service.cs", tmp);
                    }
                }
            }

            #endregion

            #region Application
            // Generate application project
            ExecuteProcess("dotnet", $"new classlib -n {_settings.ProjectName}.Application");
            ExecuteProcess("dotnet", $"sln {_settings.ProjectName}.sln add {_settings.ProjectName}.Application");
            File.Delete($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Class1.cs");

            // Create application default directories
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/UseCases");
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Interfaces");
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Ports");
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Validations");

            // Add validation class
            templateValidation = File.ReadAllText(Path.Combine(_currentPath, "Templates/BaseExceptionValidation.txt"));
            templateValidation = templateValidation
                .Replace("{{YourNamespace}}", $"{_settings.ProjectName}.Application.Validations")
                .Replace("{{Type}}", "UseCase");
            File.WriteAllText($"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Validations/UseCaseException.cs", templateValidation);

            // If there are CRUD use cases
            if (_settings.CRUDEntities != null)
            {
                // Use case interface templates
                string templateICreate = File.ReadAllText(Path.Combine(_currentPath, "Templates/ICreateEntityUseCase.txt"));
                string templateIUpdate = File.ReadAllText(Path.Combine(_currentPath, "Templates/IUpdateEntityUseCase.txt"));
                string templateIDelete = File.ReadAllText(Path.Combine(_currentPath, "Templates/IDeleteEntityUseCase.txt"));
                string templateIReadOne = File.ReadAllText(Path.Combine(_currentPath, "Templates/IReadOneEntityUseCase.txt"));
                string templateIReadMany = File.ReadAllText(Path.Combine(_currentPath, "Templates/IReadManyEntityUseCase.txt"));

                // Use case interface templates
                string templateCreate = File.ReadAllText(Path.Combine(_currentPath, "Templates/CreateEntityUseCase.txt"));
                string templateUpdate = File.ReadAllText(Path.Combine(_currentPath, "Templates/UpdateEntityUseCase.txt"));
                string templateDelete = File.ReadAllText(Path.Combine(_currentPath, "Templates/DeleteEntityUseCase.txt"));
                string templateReadOne = File.ReadAllText(Path.Combine(_currentPath, "Templates/ReadOneEntityUseCase.txt"));
                string templateReadMany = File.ReadAllText(Path.Combine(_currentPath, "Templates/ReadManyEntityUseCase.txt"));

                // Template for the input and output ports
                string templatePort = File.ReadAllText($"{_currentPath}/Templates/UseCasePort.txt");

                string entityName;
                foreach (var entity in _settings.CRUDEntities)
                {
                    // Capitalize the first letter of the entity
                    entityName = entity[..1].ToUpper() + entity[1..];

                    // Create entity usecase
                    AddUseCaseTemplate(templateICreate, templateCreate, templatePort, entityName, "Create");

                    // Update entity usecase
                    AddUseCaseTemplate(templateIUpdate, templateUpdate, templatePort, entityName, "Update");

                    // Delete entity usecase
                    AddUseCaseTemplate(templateIDelete, templateDelete, templatePort, entityName, "Delete");

                    // ReadOne entity usecase
                    AddUseCaseTemplate(templateIReadOne, templateReadOne, templatePort, entityName, "ReadOne");

                    // ReadMany entity usecase
                    AddUseCaseTemplate(templateIReadMany, templateReadMany, templatePort, entityName, "ReadMany");
                }
            }
            #endregion

            #region Infrastructure
            // Change to the Infrastructure folder
            Directory.CreateDirectory($"{_settings.ProjectPath}/src/Infrastructure");
            Directory.SetCurrentDirectory($"{_settings.ProjectPath}/src/Infrastructure");

            // Generate infrastructure (IoC) project
            ExecuteProcess("dotnet", $"new classlib -n {_settings.ProjectName}.IoC");
            ExecuteProcess("dotnet", $"sln ../{_settings.ProjectName}.sln add {_settings.ProjectName}.IoC");
            File.Delete($"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.IoC/Class1.cs");

            // Generate infrastructure (Data) project
            ExecuteProcess("dotnet", $"new classlib -n {_settings.ProjectName}.Data");
            ExecuteProcess("dotnet", $"sln ../{_settings.ProjectName}.sln add {_settings.ProjectName}.Data");
            File.Delete($"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.Data/Class1.cs");

            // If there are External Services
            if (_settings.HasExternalServices)
            {
                // Generate infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", $"new classlib -n {_settings.ProjectName}.ExternalServices");
                ExecuteProcess("dotnet", $"sln ../{_settings.ProjectName}.sln add {_settings.ProjectName}.ExternalServices");
                File.Delete($"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.ExternalServices/Class1.cs");
            }

            // If there is UI
            if (_settings.TypeUI != null)
            {
                // Set the UI project name
                _settings.NameUI = _settings.TypeUI switch
                {
                    "grpc" => "GRPC",
                    "webapi" => "WebAPI",
                    "webapp" => "WebApp",
                    "mvc" => "MVC",
                    "console" => "Console",
                    "angular" => "Angular",
                    "react" => "React",
                    _ => _settings.TypeUI
                };

                // Generate infrastructure (UI) project
                ExecuteProcess("dotnet", $"new {_settings.TypeUI} -n {_settings.ProjectName}.{_settings.NameUI}");
                ExecuteProcess("dotnet", $"sln ../{_settings.ProjectName}.sln add {_settings.ProjectName}.{_settings.NameUI}");
                File.Delete($"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.{_settings.NameUI}/Class1.cs");
            }
            #endregion

            #endregion Project Creation

            #region Add References

            // Path to projects
            string domainProjet = $"{_settings.ProjectPath}/src/{_settings.ProjectName}.Domain/{_settings.ProjectName}.Domain.csproj";
            string appProjet = $"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/{_settings.ProjectName}.Application.csproj";
            string iocProjet = $"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.IoC/{_settings.ProjectName}.IoC.csproj";
            string dataProjet = $"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.Data/{_settings.ProjectName}.Data.csproj";

            // Add a reference from the domain project to the application project
            ExecuteProcess("dotnet", $"add {appProjet} reference {domainProjet}");

            // Add a reference from the domain project to the data project
            ExecuteProcess("dotnet", $"add {dataProjet} reference {domainProjet}");

            // Add a reference from the domain project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", $"add {iocProjet} reference {domainProjet}");

            // Add a reference from the application project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", $"add {iocProjet} reference {appProjet}");

            // Add a reference from the data project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", $"add {iocProjet} reference {dataProjet}");

            // If there are External Services
            if (_settings.HasExternalServices)
            {
                string externalServicesProjet = $"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.ExternalServices/{_settings.ProjectName}.ExternalServices.csproj";

                // Add a reference from the domain project to the infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", $"add {externalServicesProjet} reference {domainProjet}");

                // Add a reference from the external services project to the infrastructure (IoC) project
                ExecuteProcess("dotnet", $"add {iocProjet} reference {externalServicesProjet}");
            }

            // If there is UI
            if (_settings.TypeUI != null)
            {
                string uiProject = $"{_settings.ProjectPath}/src/Infrastructure/{_settings.ProjectName}.{_settings.NameUI}/{_settings.ProjectName}.{_settings.NameUI}.csproj";

                // Add a reference from the application project to the infrastructure (UI) project
                ExecuteProcess("dotnet", $"add {uiProject} reference {appProjet}");

                // Add a reference from the IoC project to the infrastructure (UI) project
                ExecuteProcess("dotnet", $"add {uiProject} reference {iocProjet}");
            }

            #endregion Add References
        }

        /// <summary>
        /// Add usecase templates for a CRUD operation.
        /// </summary>
        /// <param name="templateInterface">Template for the usecase interface.</param>
        /// <param name="templateUseCase">Template for the usecase.</param>
        /// <param name="templatePort">Template for the input and output ports.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="type">Type of the usecase.</param>
        private void AddUseCaseTemplate(string templateInterface, string templateUseCase, string templatePort, string entityName, string type)
        {
            // Generate directories path
            string directoryIU = $"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Interfaces/UseCases/{entityName}";
            string directoryU = $"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/UseCases/{entityName}";
            string directoryP = $"{_settings.ProjectPath}/src/{_settings.ProjectName}.Application/Ports/{entityName}";

            // Create directories if they don't exist
            Directory.CreateDirectory(directoryIU);
            Directory.CreateDirectory(directoryU);
            Directory.CreateDirectory(directoryP);

            // Generate namespaces
            string iuNamespace = $"{_settings.ProjectName}.Application.Interfaces.UseCases.{entityName}";
            string uNamespace = $"{_settings.ProjectName}.Application.UseCases.{entityName}";
            string pNamespace = $"{_settings.ProjectName}.Application.Ports.{entityName}";
            // string entityNamespace = $"{_settings.ProjectName}.Domain.Entities";
            string repositoryNamespace = $"{_settings.ProjectName}.Domain.Interfaces.Repositories";

            #region Interface UseCase
            // Generate usecase template
            string tmp = templateInterface
                .Replace("{{YourNamespace}}", iuNamespace)
                .Replace("{{PortNamespace}}", pNamespace)
                .Replace("{{EntityName}}", entityName)
                .Replace("{{InputName}}", $"{type}{entityName}Input")
                .Replace("{{OutputName}}", $"Read{entityName}Output");

            // Add usecase in usecase directory
            File.WriteAllText($"{directoryIU}/{type}{entityName}UseCase.cs", tmp);
            #endregion

            #region UseCase Ports
            // Create input port all entities unless for Read operations
            if (!type.Contains("Read"))
            {
                // Create input port
                tmp = templatePort
                    .Replace("{{YourNamespace}}", pNamespace)
                    .Replace("{{EntityName}}", entityName)
                    .Replace("{{PortName}}", $"{type}{entityName}Input");

                // Add input port in usecase directory
                File.WriteAllText($"{directoryP}/{type}{entityName}Input.cs", tmp);
            }
            else
            {
                // Create output port
                tmp = templatePort
                    .Replace("{{YourNamespace}}", pNamespace)
                    .Replace("{{EntityName}}", entityName)
                    .Replace("{{PortName}}", $"Read{entityName}Output");

                // Add output port in usecase directory
                File.WriteAllText($"{directoryP}/Read{entityName}Output.cs", tmp);
            }
            #endregion

            #region UseCase
            // Generate usecase template
            tmp = templateUseCase
                .Replace("{{YourNamespace}}", uNamespace)
                .Replace("{{InterfaceNamespace}}", iuNamespace)
                .Replace("{{PortNamespace}}", pNamespace)
                .Replace("{{RepositoryNamespace}}", repositoryNamespace)
                .Replace("{{EntityName}}", entityName)
                .Replace("{{InputName}}", $"{type}{entityName}Input")
                .Replace("{{OutputName}}", $"Read{entityName}Output");

            // Add usecase in usecase directory
            File.WriteAllText($"{directoryU}/{type}{entityName}UseCase.cs", tmp);
            #endregion
        }

        /// <summary>
        /// Executes a process and waits for it to finish.
        /// </summary>
        /// <param name="fileName">The name of the file to execute.</param>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="Exception">Thrown when the process returns an error.</exception>
        private static void ExecuteProcess(string fileName, string command)
        {
            // Create a process to execute the command
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = command,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            // Start the process
            process.Start();

            // Wait for the process to finish
            process.WaitForExit();

            // Check the exit code to see if the process completed successfully
            int exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                throw new Exception($"Error: {exitCode}");
            }

            // Close the process and release resources
            process.Close();
        }
    }
}