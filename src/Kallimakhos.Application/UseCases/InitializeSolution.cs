using Kallimakhos.Application.Interfaces;
using Kallimakhos.Application.Ports;
using Kallimakhos.Domain.Entities;

namespace Kallimakhos.Application.UseCases
{
    public class InitializeSolution : IInitializeSolution
    {
        public void Execute(SettingsInput settingsInput)
        {
            Console.WriteLine("Initializing solution...");
            var solution = new Solution(
                projectName: settingsInput.ProjectName,
                projectPath: settingsInput.ProjectPath
            );
            solution.Initialize();
        }
    }
}