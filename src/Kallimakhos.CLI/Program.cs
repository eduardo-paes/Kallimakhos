using Kallimakhos.CLI.Factories;
using Kallimakhos.IoC.Factories;

namespace Kallimakhos.CLI
{
    public static class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments passed to the program.</param>
        public static void Main(string[] args)
        {
            try
            {
                // Add a help function
                if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
                {
                    DisplayHelpMenuFactory.Create().Execute();
                    return;
                }

                // Execute the command
                var input = args[0].ToLower() switch
                {
                    "-np" => GetArgsFromCliFactory.Create().Execute(args),
                    "start" => GetArgsInteractivelyFactory.Create().Execute(),
                    _ => throw new Exception("Invalid command."),
                };

                // Execute the use cases
                ApplicationFactories.CreateInitializeSolution().Execute(input);
                ApplicationFactories.CreateAddDomainLayer().Execute(input);
                ApplicationFactories.CreateAddApplicationLayer().Execute(input);
                ApplicationFactories.CreateAddInfrastructureLayer().Execute(input);

                // Display success message
                Console.WriteLine("Project created successfully.");
                Console.WriteLine("Path to project: " + Path.Combine(input.ProjectPath!, input.ProjectName!));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}