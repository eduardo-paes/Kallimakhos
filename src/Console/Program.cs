using System.Diagnostics;
using System.Globalization;

static class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Arguments passed to the program.</param>
    /// <remarks>
    /// Arguments:
    ///  <PATH>       Path where the project will be created.
    ///  <PROJECT_NAME>   Project name.
    /// -es          Add external services project.
    /// -ui          Add UI project. Valid types: grpc, webapi, webapp, mvc, console, angular, react
    /// </remarks>
    public static void Main(string[] args)
    {
        try
        {
            #region Parameter Initialization

            // Add help function
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                Console.WriteLine("======================================================================");
                Console.WriteLine("Clean Architecture Project Creator");
                Console.WriteLine("======================================================================");
                Console.WriteLine();
                Console.WriteLine("Usage: <PATH> <PROJECT_NAME> [-es] [-ui <UI_TYPE>]");
                Console.WriteLine("Creates a project with clean architecture.");
                Console.WriteLine();
                Console.WriteLine("Arguments:");
                Console.WriteLine("  <PATH>       Path where the project will be created.");
                Console.WriteLine("  <PROJECT_NAME>   Project name.");
                Console.WriteLine("  -es          Add external services project.");
                Console.WriteLine("  -ui          Add UI project. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                return;
            }

            // Get where the project will be saved
            string path = args[0];

            // Check if the path is valid
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid path.");
                return;
            }

            // Get the project name
            string name = args[1];

            // Check if the project name is valid
            if (name.Length == 0)
            {
                Console.WriteLine("Invalid project name.");
                return;
            }

            // Create a path for the project
            string projectPath = Path.Combine(path, name);

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
                switch (args[index + 1].ToLower())
                {
                    case "grpc":
                        typeUI = "grpc";
                        break;
                    case "webapi":
                        typeUI = "webapi";
                        break;
                    case "webapp":
                        typeUI = "webapp";
                        break;
                    case "mvc":
                        typeUI = "mvc";
                        break;
                    case "console":
                        typeUI = "console";
                        break;
                    case "angular":
                        typeUI = "angular";
                        break;
                    case "react":
                        typeUI = "react";
                        break;
                    default:
                        Console.WriteLine("Invalid UI type. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                        return;
                }

                // Check if the UI type is valid
                if (typeUI == null)
                {
                    Console.WriteLine("Invalid UI type. Valid types: grpc, webapi, webapp, mvc, console, angular, react");
                    return;
                }

                // Convert UI type to PascalCase
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                typeUI = textInfo.ToTitleCase(typeUI).Replace(" ", "");
            }

            #endregion Parameter Initialization

            #region Project Initialization

            // Create a folder for the project
            Directory.CreateDirectory(projectPath);

            // Add readme file
            File.WriteAllText(projectPath + "/README.md", "# " + name);

            // Create a folder for the project source code
            Directory.CreateDirectory(projectPath + "/src");

            // Change to the project folder
            Directory.SetCurrentDirectory(projectPath);

            // Execute command to generate a .gitignore file
            ExecuteProcess("dotnet", "new gitignore");

            // Change to the source folder
            Directory.SetCurrentDirectory(projectPath + "/src");

            // Generate the project solution
            ExecuteProcess("dotnet", "new sln --name " + name);

            #endregion Project Initialization

            #region Project Creation

            // Generate domain project
            ExecuteProcess("dotnet", "new classlib -n Domain");

            // Generate application project
            ExecuteProcess("dotnet", "new classlib -n Application");

            // Change to the Infrastructure folder
            Directory.CreateDirectory(projectPath + "/src/Infrastructure");
            Directory.SetCurrentDirectory(projectPath + "/src/Infrastructure");

            // Generate infrastructure (IoC) project
            ExecuteProcess("dotnet", "new classlib -n IoC");

            // Generate infrastructure (Data) project
            ExecuteProcess("dotnet", "new classlib -n Data");

            // If there are External Services
            if (hasExternalServices)
            {
                // Generate infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", "new classlib -n ExternalServices");
            }

            // If there is UI
            if (typeUI != null)
            {
                // Generate infrastructure (UI) project
                ExecuteProcess("dotnet", "new classlib -n " + typeUI);
            }

            #endregion Project Creation

            #region Add References

            // Add a reference from the domain project to the application project
            ExecuteProcess("dotnet", "add " + projectPath + "/src/Application/Application.csproj reference " + projectPath + "/src/Domain/Domain.csproj");

            // Add a reference from the domain project to the data project
            ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/Data/Data.csproj reference " + projectPath + "/src/Domain/Domain.csproj");

            // Add a reference from the domain project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/IoC/IoC.csproj reference " + projectPath + "/src/Domain/Domain.csproj");

            // Add a reference from the application project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/IoC/IoC.csproj reference " + projectPath + "/src/Application/Application.csproj");

            // Add a reference from the data project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/IoC/IoC.csproj reference " + projectPath + "/src/Infrastructure/Data/Data.csproj");

            // If there are External Services
            if (hasExternalServices)
            {
                // Add a reference from the domain project to the infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/ExternalServices/ExternalServices.csproj reference " + projectPath + "/src/Domain/Domain.csproj");

                // Add a reference from the external services project to the infrastructure (IoC) project
                ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/IoC/IoC.csproj reference " + projectPath + "/src/Infrastructure/ExternalServices/ExternalServices.csproj");
            }

            // If there is UI
            if (typeUI != null)
            {
                // Add a reference from the application project to the infrastructure (UI) project
                ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/" + typeUI + "/" + typeUI + ".csproj reference " + projectPath + "/src/Application/Application.csproj");

                // Add a reference from the IoC project to the infrastructure (UI) project
                ExecuteProcess("dotnet", "add " + projectPath + "/src/Infrastructure/" + typeUI + "/" + typeUI + ".csproj reference " + projectPath + "/src/Infrastructure/IoC/IoC.csproj");
            }

            #endregion Add References
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(-1);
        }
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
