using System.Globalization;
using NewCleanArch.Interfaces;
using NewCleanArch.Services;

namespace NewCleanArch.Factories
{
    public class ProjectServiceFactory
    {
        public static ICreateProjectService Execute(string[] args)
        {
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

            // Get the project name
            string projectName = args[2];

            // Check if the project name is valid
            if (projectName.Length == 0)
            {
                throw new Exception("Invalid project name.");
            }

            // Create a path for the project
            string projectPath = Path.Combine(path, projectName);

            // Check if the user wants External Services (-es)
            bool hasExternalServices = false;
            int index = Array.IndexOf(args, "-es");
            if (index > 0)
            {
                hasExternalServices = true;
            }

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

            // Return the service
            return new CreateProjectService(projectName, projectPath, hasExternalServices, typeUI);
        }
    }
}