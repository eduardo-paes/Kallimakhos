using Kallimakhos.Application.Interfaces;
using Kallimakhos.Application.Ports;
using Kallimakhos.Entities.Domain;

namespace Kallimakhos.Application.UseCases
{
    public class AddApplicationLayer : IAddApplicationLayer
    {
        public void Execute(SettingsInput settingsInput)
        {
            Console.WriteLine("Adding application layer...");

            // Create application project
            var applicationProject = new ApplicationProject(
                projectName: settingsInput.ProjectName,
                projectPath: settingsInput.ProjectPath
            );

            // Add application layer to solution
            applicationProject.AddLayer();

            // Add use cases to application layer
            applicationProject.AddCRUDUseCases(settingsInput.CRUDEntities);
        }
    }
}