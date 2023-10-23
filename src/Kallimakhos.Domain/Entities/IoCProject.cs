using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class IoCProject : BaseEntity
    {
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        public IoCProject(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
        }

        /// <summary>
        /// Add the IoC project to the solution.
        /// </summary>
        /// <param name="dataProject">Path to the data project.</param>
        /// <param name="externalServicesProject">Path to the external services project.</param>
        /// <param name="uiProject">Path to the UI project.</param>
        public void AddLayer(string? dataProject, string? externalServicesProject, string? uiProject)
        {
            // Generate IoC project path
            string? iocProjet = $"{ProjectPath}/src/Infrastructure/{ProjectName}.IoC/{ProjectName}.IoC.csproj";

            // Generate infrastructure (IoC) project
            ExecuteProcess("dotnet", $"new classlib -n {ProjectName}.IoC");
            ExecuteProcess("dotnet", $"sln ../{ProjectName}.sln add {ProjectName}.IoC");
            File.Delete($"{ProjectPath}/src/Infrastructure/{ProjectName}.IoC/Class1.cs");

            // Get domain project path
            string domainProjet = $"{ProjectPath}/src/{ProjectName}.Domain/{ProjectName}.Domain.csproj";

            // Add a reference from the domain project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", $"add {iocProjet} reference {domainProjet}");
            // TODO: Add DI for the domain project

            // Get application project path
            string appProjet = $"{ProjectPath}/src/{ProjectName}.Application/{ProjectName}.Application.csproj";

            // Add a reference from the application project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", $"add {iocProjet} reference {appProjet}");
            // TODO: Add DI for the application project

            // Add a reference from the data project to the infrastructure (IoC) project
            if (!string.IsNullOrEmpty(dataProject))
            {
                ExecuteProcess("dotnet", $"add {iocProjet} reference {dataProject}");
                // TODO: Add DI for the data project
            }

            // Add a reference from the external services project to the infrastructure (IoC) project
            if (!string.IsNullOrEmpty(externalServicesProject))
            {
                ExecuteProcess("dotnet", $"add {iocProjet} reference {externalServicesProject}");
                // TODO: Add DI for the external services project
            }

            // Add a reference from the UI project to the infrastructure (IoC) project
            if (!string.IsNullOrEmpty(uiProject))
            {
                ExecuteProcess("dotnet", $"add {uiProject} reference {iocProjet}");
            }
        }
    }
}