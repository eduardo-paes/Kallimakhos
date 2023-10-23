using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class ExternalServiceProject : BaseEntity
    {
        public string? CurrentPath { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }
        public string? ExternalServicesProjectPath { get; set; }

        public ExternalServiceProject(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            CurrentPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Add the external services project to the solution.
        /// </summary>
        public void AddLayer()
        {
            // Generate infrastructure (ExternalServices) project
            ExecuteProcess("dotnet", $"new classlib -n {ProjectName}.ExternalServices");
            ExecuteProcess("dotnet", $"sln ../{ProjectName}.sln add {ProjectName}.ExternalServices");
            File.Delete($"{ProjectPath}/src/Infrastructure/{ProjectName}.ExternalServices/Class1.cs");

            // Get external services project path
            ExternalServicesProjectPath = $"{ProjectPath}/src/Infrastructure/{ProjectName}.ExternalServices/{ProjectName}.ExternalServices.csproj";

            // Generate domain project path
            string domainProjet = $"{ProjectPath}/src/{ProjectName}.Domain/{ProjectName}.Domain.csproj";

            // Add a reference from the domain project to the infrastructure (ExternalServices) project
            ExecuteProcess("dotnet", $"add {ExternalServicesProjectPath} reference {domainProjet}");

            // TODO: Create services
        }
    }
}