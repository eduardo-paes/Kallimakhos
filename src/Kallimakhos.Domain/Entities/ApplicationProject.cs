using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class ApplicationProject : BaseEntity
    {
        public string? CurrentPath { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        #region DI References
        private readonly Dictionary<string, string> UseCasesToDI = new();
        private readonly Dictionary<string, string> NamespacesToDI = new();
        #endregion

        public ApplicationProject(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            CurrentPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Add the application project to the solution.
        /// </summary>
        public void AddLayer()
        {
            // Generate application project
            ExecuteProcess("dotnet", $"new classlib -n {ProjectName}.Application");
            ExecuteProcess("dotnet", $"sln {ProjectName}.sln add {ProjectName}.Application");
            File.Delete($"{ProjectPath}/src/{ProjectName}.Application/Class1.cs");

            // Create application default directories
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Application/UseCases");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Application/Interfaces");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Application/Ports");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Application/Validations");

            // Add validation class
            string templateValidation = File.ReadAllText(Path.Combine(CurrentPath, "Templates/BaseExceptionValidation.txt"));
            templateValidation = templateValidation
                .Replace("{{YourNamespace}}", $"{ProjectName}.Application.Validations")
                .Replace("{{Type}}", "UseCase");
            File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Application/Validations/UseCaseException.cs", templateValidation);

            // Add references
            string domainProjet = $"{ProjectPath}/src/{ProjectName}.Domain/{ProjectName}.Domain.csproj";
            string appProjet = $"{ProjectPath}/src/{ProjectName}.Application/{ProjectName}.Application.csproj";

            // Add a reference from the domain project to the application project
            ExecuteProcess("dotnet", $"add {appProjet} reference {domainProjet}");
        }

        /// <summary>
        /// Add usecases to the application project.
        /// </summary>
        /// <param name="crudEntities">The entities to be added.</param>
        public void AddCRUDUseCases(string[]? crudEntities)
        {
            // If there are CRUD use cases
            if (crudEntities != null)
            {
                // Use case interface templates
                string templateICreate = File.ReadAllText(Path.Combine(CurrentPath, "Templates/ICreateEntityUseCase.txt"));
                string templateIUpdate = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IUpdateEntityUseCase.txt"));
                string templateIDelete = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IDeleteEntityUseCase.txt"));
                string templateIReadOne = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IReadOneEntityUseCase.txt"));
                string templateIReadMany = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IReadManyEntityUseCase.txt"));

                // Use case interface templates
                string templateCreate = File.ReadAllText(Path.Combine(CurrentPath, "Templates/CreateEntityUseCase.txt"));
                string templateUpdate = File.ReadAllText(Path.Combine(CurrentPath, "Templates/UpdateEntityUseCase.txt"));
                string templateDelete = File.ReadAllText(Path.Combine(CurrentPath, "Templates/DeleteEntityUseCase.txt"));
                string templateReadOne = File.ReadAllText(Path.Combine(CurrentPath, "Templates/ReadOneEntityUseCase.txt"));
                string templateReadMany = File.ReadAllText(Path.Combine(CurrentPath, "Templates/ReadManyEntityUseCase.txt"));

                // Template for the input and output ports
                string templatePort = File.ReadAllText($"{CurrentPath}/Templates/UseCasePort.txt");

                string entityName;
                foreach (var entity in crudEntities)
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
            string directoryIU = $"{ProjectPath}/src/{ProjectName}.Application/Interfaces/UseCases/{entityName}";
            string directoryU = $"{ProjectPath}/src/{ProjectName}.Application/UseCases/{entityName}";
            string directoryP = $"{ProjectPath}/src/{ProjectName}.Application/Ports/{entityName}";

            // Create directories if they don't exist
            Directory.CreateDirectory(directoryIU);
            Directory.CreateDirectory(directoryU);
            Directory.CreateDirectory(directoryP);

            // Generate namespaces
            string iuNamespace = $"{ProjectName}.Application.Interfaces.UseCases.{entityName}";
            string uNamespace = $"{ProjectName}.Application.UseCases.{entityName}";
            string pNamespace = $"{ProjectName}.Application.Ports.{entityName}";
            // string entityNamespace = $"{ProjectName}.Domain.Entities";
            string repositoryNamespace = $"{ProjectName}.Domain.Interfaces.Repositories";

            // Add usecases, interfaces and its namespaces to be added to DI references in IoC project
            UseCasesToDI.Add($"I{type}{entityName}UseCase", $"{type}{entityName}UseCase");
            if (!NamespacesToDI.ContainsKey(iuNamespace))
                NamespacesToDI.Add(iuNamespace, iuNamespace);

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
    }
}