using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Domain.Entities
{
    public class Solution : BaseEntity
    {
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        public Solution(string? projectName, string? projectPath)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
        }

        /// <summary>
        /// Initialize the solution.
        /// </summary>
        public void Initialize()
        {
            // Create a folder for the project
            Directory.CreateDirectory(ProjectPath);

            // Add readme file
            File.WriteAllText($"{ProjectPath}/README.md", $"# {ProjectName}");

            // Create a folder for the project source code
            Directory.CreateDirectory($"{ProjectPath}/src");

            // Change to the project folder
            Directory.SetCurrentDirectory(ProjectPath);

            // Execute command to generate a .gitignore file
            ExecuteProcess("dotnet", $"new gitignore");

            // Change to the source folder
            Directory.SetCurrentDirectory($"{ProjectPath}/src");

            // Generate the project solution
            ExecuteProcess("dotnet", $"new sln --name {ProjectName}");
        }

        /// <summary>
        /// Add the infrastructure folder to the solution.
        /// </summary>
        public void AddInfraLayer()
        {
            // Change to the Infrastructure folder
            Directory.CreateDirectory($"{ProjectPath}/src/Infrastructure");
            Directory.SetCurrentDirectory($"{ProjectPath}/src/Infrastructure");
        }
    }
}