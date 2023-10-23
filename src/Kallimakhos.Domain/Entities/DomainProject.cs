using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class DomainProject : BaseEntity
    {
        public string? CurrentPath { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        #region DI References
        private readonly Dictionary<string, string> UseCasesToDI = new();
        private readonly Dictionary<string, string> NamespacesToDI = new();
        #endregion

        public DomainProject(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            CurrentPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Add the domain project to the solution.
        /// </summary>
        public void AddLayer()
        {
            // Generate domain project
            ExecuteProcess("dotnet", $"new classlib -n {ProjectName}.Domain");
            ExecuteProcess("dotnet", $"sln {ProjectName}.sln add {ProjectName}.Domain");
            File.Delete($"{ProjectPath}/src/{ProjectName}.Domain/Class1.cs");
        }

        /// <summary>
        /// Add entities to the domain project.
        /// </summary>
        /// <param name="entityNames">The entities to be added.</param>
        public void AddEntities(string[]? entityNames)
        {
            // Create domain default directories
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Entities");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Validations");

            // Add validation class
            string templateValidation = File.ReadAllText(Path.Combine(CurrentPath, "Templates/BaseExceptionValidation.txt"));
            templateValidation = templateValidation
                .Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Validations")
                .Replace("{{Type}}", "Entity");
            File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Validations/EntityException.cs", templateValidation);

            // If entities were provided
            if (entityNames != null)
            {
                string template = File.ReadAllText(Path.Combine(CurrentPath, "Templates/Entity.txt"));
                template = template.Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Entities");

                // Create entities with capital letters
                string entityName, tmp;
                foreach (var entity in entityNames)
                {
                    // Capitalize the first letter of the entity
                    entityName = entity[..1].ToUpper() + entity[1..];

                    // Create the repository file using the template
                    tmp = template.Replace("{{EntityName}}", entityName);
                    File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Entities/{entityName}.cs", tmp);
                }
            }
        }

        /// <summary>
        /// Add repositories to the domain project.
        /// </summary>
        /// <param name="entityNames">The entities to be added.</param>
        /// <param name="crudEntities">The entities to be added with CRUD operations.</param>
        /// <param name="hasCRUD">If the project has CRUD operations.</param>
        public void AddRepositories(string[]? entityNames, string[]? crudEntities, bool hasCRUD)
        {
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Repositories");

            // Convert the entities to a list of strings
            List<string> entities = new();
            if (entityNames != null)
            {
                entities = entityNames.ToList();
            }

            // If there are CRUDs
            if (hasCRUD)
            {
                // Create a folder for the base repository
                Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Repositories/Base");

                // Create a base repository with CRUD operations
                string template = File.ReadAllText(Path.Combine(CurrentPath, "Templates/ICrudRepository.txt"));
                template = template.Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Interfaces.Repositories.Base");
                File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Repositories/Base/ICrudRepository.cs", template);

                // If CRUD entities were provided
                if (crudEntities != null)
                {
                    // Create repositories with CRUD operations
                    template = File.ReadAllText(Path.Combine(CurrentPath, "Templates/ICrudEntityRepository.txt"));
                    template = template
                        .Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Interfaces.Repositories")
                        .Replace("{{ProjectName}}", $"{ProjectName}");

                    // Create repositories with CRUD operations
                    string entityName, tmp;
                    foreach (var entity in crudEntities)
                    {
                        // Remove the entity from the list
                        entities.Remove(entity);

                        // Capitalize the first letter of the entity
                        entityName = entity[..1].ToUpper() + entity[1..];

                        // Create the repository file using the template
                        tmp = template.Replace("{{EntityName}}", entityName);
                        File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Repositories/I{entityName}Repository.cs", tmp);
                    }
                }
            }

            // If entities were provided
            if (entities.Count > 0)
            {
                // Create repositories with CRUD operations
                string template = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IEntityRepository.txt"));
                template = template.Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Interfaces.Repositories");

                // Create repositories
                string entityName, tmp;
                foreach (var entity in entities)
                {
                    // Capitalize the first letter of the entity
                    entityName = entity[..1].ToUpper() + entity[1..];

                    // Create the repository file using the template
                    tmp = template.Replace("{{EntityName}}", entityName);
                    File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Repositories/I{entityName}Repository.cs", tmp);
                }
            }
        }

        /// <summary>
        /// Add services to the domain project.
        /// </summary>
        /// <param name="nameServices">The services to be added.</param>
        public void AddExternalServices(string[]? nameServices)
        {
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces");
            Directory.CreateDirectory($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Services");

            // If services were provided
            if (nameServices != null)
            {
                // Create services
                string template = File.ReadAllText(Path.Combine(CurrentPath, "Templates/IService.txt"));
                template = template.Replace("{{YourNamespace}}", $"{ProjectName}.Domain.Interfaces.Services");

                // Create services
                string serviceName, tmp;
                foreach (var service in nameServices)
                {
                    // Capitalize the first letter of the service
                    serviceName = service[..1].ToUpper() + service[1..];

                    // Create the repository file using the template
                    tmp = template.Replace("{{ServiceName}}", serviceName);
                    File.WriteAllText($"{ProjectPath}/src/{ProjectName}.Domain/Interfaces/Services/I{serviceName}Service.cs", tmp);
                }
            }
        }
    }
}