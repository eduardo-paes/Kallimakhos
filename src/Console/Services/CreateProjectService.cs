using System.Diagnostics;
using NewCleanArch.Interfaces;

namespace NewCleanArch.Services
{
    public class CreateProjectService : ICreateProjectService
    {
        /// <summary>
        /// The name of the project.
        /// </summary>
        private string _projectName { get; set; }

        /// <summary>
        /// The path where the project will be created.
        /// </summary>
        private string _projectPath { get; set; }

        /// <summary>
        /// Indicates if the project has external services.
        /// </summary>
        private bool _hasExternalServices { get; set; }

        /// <summary>
        /// The type of UI.
        /// </summary>
        private string? _typeUI { get; set; }

        /// <summary>
        /// Name of UI project.
        /// </summary>
        private string? _uiName { get; set; }

        public CreateProjectService(string projectName, string projectPath, bool hasExternalServices, string? typeUI)
        {
            _projectName = projectName;
            _projectPath = projectPath;
            _hasExternalServices = hasExternalServices;
            _typeUI = typeUI;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public void Execute()
        {
            #region Project Initialization

            // Create a folder for the project
            Directory.CreateDirectory(_projectPath);

            // Add readme file
            File.WriteAllText(_projectPath + "/README.md", "# " + _projectName);

            // Create a folder for the project source code
            Directory.CreateDirectory(_projectPath + "/src");

            // Change to the project folder
            Directory.SetCurrentDirectory(_projectPath);

            // Execute command to generate a .gitignore file
            ExecuteProcess("dotnet", "new gitignore");

            // Change to the source folder
            Directory.SetCurrentDirectory(_projectPath + "/src");

            // Generate the project solution
            ExecuteProcess("dotnet", "new sln --name " + _projectName);

            #endregion Project Initialization

            #region Project Creation

            // Generate domain project
            ExecuteProcess("dotnet", "new classlib -n " + _projectName + ".Domain");
            ExecuteProcess("dotnet", "sln " + _projectName + ".sln add " + _projectName + ".Domain");
            File.Delete(_projectPath + "/src/" + _projectName + ".Domain/Class1.cs");

            // Generate application project
            ExecuteProcess("dotnet", "new classlib -n " + _projectName + ".Application");
            ExecuteProcess("dotnet", "sln " + _projectName + ".sln add " + _projectName + ".Application");
            File.Delete(_projectPath + "/src/" + _projectName + ".Application/Class1.cs");

            // Change to the Infrastructure folder
            Directory.CreateDirectory(_projectPath + "/src/Infrastructure");
            Directory.SetCurrentDirectory(_projectPath + "/src/Infrastructure");

            // Generate infrastructure (IoC) project
            ExecuteProcess("dotnet", "new classlib -n " + _projectName + ".IoC");
            ExecuteProcess("dotnet", "sln ../" + _projectName + ".sln add " + _projectName + ".IoC");
            File.Delete(_projectPath + "/src/Infrastructure/" + _projectName + ".IoC/Class1.cs");

            // Generate infrastructure (Data) project
            ExecuteProcess("dotnet", "new classlib -n " + _projectName + ".Data");
            ExecuteProcess("dotnet", "sln ../" + _projectName + ".sln add " + _projectName + ".Data");
            File.Delete(_projectPath + "/src/Infrastructure/" + _projectName + ".Data/Class1.cs");

            // If there are External Services
            if (_hasExternalServices)
            {
                // Generate infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", "new classlib -n " + _projectName + ".ExternalServices");
                ExecuteProcess("dotnet", "sln ../" + _projectName + ".sln add " + _projectName + ".ExternalServices");
                File.Delete(_projectPath + "/src/Infrastructure/" + _projectName + ".ExternalServices/Class1.cs");
            }

            // If there is UI
            if (_typeUI != null)
            {
                SetUiProjectName();

                // Generate infrastructure (UI) project
                ExecuteProcess("dotnet", "new " + _typeUI + " -n " + _projectName + "." + _uiName);
                ExecuteProcess("dotnet", "sln ../" + _projectName + ".sln add " + _projectName + "." + _uiName);
                File.Delete(_projectPath + "/src/Infrastructure/" + _projectName + "." + _uiName + "/Class1.cs");
            }

            #endregion Project Creation

            #region Add References

            // Path to projects
            string domainProjet = _projectPath + "/src/" + _projectName + ".Domain/" + _projectName + ".Domain.csproj";
            string appProjet = _projectPath + "/src/" + _projectName + ".Application/" + _projectName + ".Application.csproj";
            string iocProjet = _projectPath + "/src/Infrastructure/" + _projectName + ".IoC/" + _projectName + ".IoC.csproj";
            string dataProjet = _projectPath + "/src/Infrastructure/" + _projectName + ".Data/" + _projectName + ".Data.csproj";

            // Add a reference from the domain project to the application project
            ExecuteProcess("dotnet", "add " + appProjet + " reference " + domainProjet);

            // Add a reference from the domain project to the data project
            ExecuteProcess("dotnet", "add " + dataProjet + " reference " + domainProjet);

            // Add a reference from the domain project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + iocProjet + " reference " + domainProjet);

            // Add a reference from the application project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + iocProjet + " reference " + appProjet);

            // Add a reference from the data project to the infrastructure (IoC) project
            ExecuteProcess("dotnet", "add " + iocProjet + " reference " + dataProjet);

            // If there are External Services
            if (_hasExternalServices)
            {
                string externalServicesProjet = _projectPath + "/src/Infrastructure/" + _projectName + ".ExternalServices/" + _projectName + ".ExternalServices.csproj";

                // Add a reference from the domain project to the infrastructure (ExternalServices) project
                ExecuteProcess("dotnet", "add " + externalServicesProjet + " reference " + domainProjet);

                // Add a reference from the external services project to the infrastructure (IoC) project
                ExecuteProcess("dotnet", "add " + iocProjet + " reference " + externalServicesProjet);
            }

            // If there is UI
            if (_typeUI != null)
            {
                string uiProject = _projectPath + "/src/Infrastructure/" + _projectName + "." + _uiName + "/" + _projectName + "." + _uiName + ".csproj";

                // Add a reference from the application project to the infrastructure (UI) project
                ExecuteProcess("dotnet", "add " + uiProject + " reference " + appProjet);

                // Add a reference from the IoC project to the infrastructure (UI) project
                ExecuteProcess("dotnet", "add " + uiProject + " reference " + iocProjet);
            }

            #endregion Add References
        }

        private void SetUiProjectName()
        {
            _uiName = _typeUI switch
            {
                "grpc" => "GRPC",
                "webapi" => "WebAPI",
                "webapp" => "WebApp",
                "mvc" => "MVC",
                "console" => "Console",
                "angular" => "Angular",
                "react" => "React",
                _ => _typeUI
            };
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