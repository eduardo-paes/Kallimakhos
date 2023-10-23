using Kallimakhos.Application.Interfaces;
using Kallimakhos.Application.Ports;
using Kallimakhos.Entities.Domain;

namespace Kallimakhos.Application.UseCases
{
    public class AddDomainLayer : IAddDomainLayer
    {
        public void Execute(SettingsInput settingsInput)
        {
            Console.WriteLine("Adding domain layer...");

            // Create domain project
            var domainProject = new DomainProject(
                projectName: settingsInput.ProjectName,
                projectPath: settingsInput.ProjectPath
            );

            // Add domain layer to solution
            domainProject.AddLayer();

            // Add entities to domain layer
            if (settingsInput.HasEntities)
                domainProject.AddEntities(settingsInput.EntityNames);

            // Add repositories to domain layer
            if (settingsInput.HasRepositories)
                domainProject.AddRepositories(settingsInput.EntityNames, settingsInput.CRUDEntities, settingsInput.HasCRUDs);

            // Add external services to domain layer
            if (settingsInput.HasExternalServices)
                domainProject.AddExternalServices(settingsInput.ServiceNames);
        }
    }
}