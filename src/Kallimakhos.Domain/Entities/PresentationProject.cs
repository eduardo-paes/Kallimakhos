using Kallimakhos.Domain.Entities.Base;

namespace Kallimakhos.Entities.Domain
{
    public class PresentationProject : BaseEntity
    {
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }
        public string? UIProject { get; set; }
        public string? TypeUI { get; set; }

        public PresentationProject(string? projectName, string? projectPath, string? typeUI)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            TypeUI = typeUI;
        }

        /// <summary>
        /// Add the presentation project to the solution.
        /// </summary>
        public void AddLayer()
        {
            // Set the UI project name
            var nameUI = TypeUI switch
            {
                "grpc" => "GRPC",
                "webapi" => "WebAPI",
                "webapp" => "WebApp",
                "mvc" => "MVC",
                "console" => "Console",
                "angular" => "Angular",
                "react" => "React",
                _ => TypeUI
            };

            // Generate infrastructure (UI) project
            ExecuteProcess("dotnet", $"new {TypeUI} -n {ProjectName}.{nameUI}");
            ExecuteProcess("dotnet", $"sln ../{ProjectName}.sln add {ProjectName}.{nameUI}");
            File.Delete($"{ProjectPath}/src/Infrastructure/{ProjectName}.{nameUI}/Class1.cs");

            // Get UI project path
            UIProject = $"{ProjectPath}/src/Infrastructure/{ProjectName}.{nameUI}/{ProjectName}.{nameUI}.csproj";

            // Get application project path
            string appProjet = $"{ProjectPath}/src/{ProjectName}.Application/{ProjectName}.Application.csproj";

            // Add a reference from the application project to the infrastructure (UI) project
            ExecuteProcess("dotnet", $"add {UIProject} reference {appProjet}");
        }
    }
}