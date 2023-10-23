using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class DataProject : BaseEntity
    {
        public string? CurrentPath { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }
        public string? DataProjectPath { get; set; }

        public DataProject(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            CurrentPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Add the data project to the solution.
        /// </summary>
        public void AddLayer()
        {
            // Generate infrastructure (Data) project
            ExecuteProcess("dotnet", $"new classlib -n {ProjectName}.Data");
            ExecuteProcess("dotnet", $"sln ../{ProjectName}.sln add {ProjectName}.Data");
            File.Delete($"{ProjectPath}/src/Infrastructure/{ProjectName}.Data/Class1.cs");

            // Get data project path
            DataProjectPath = $"{ProjectPath}/src/Infrastructure/{ProjectName}.Data/{ProjectName}.Data.csproj";

            // Generate domain project path
            string domainProjet = $"{ProjectPath}/src/{ProjectName}.Domain/{ProjectName}.Domain.csproj";

            // Add a reference from the domain project to the data project
            ExecuteProcess("dotnet", $"add {DataProjectPath} reference {domainProjet}");

            // TODO: Create context, mappings and repositories
        }
    }
}