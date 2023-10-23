using Kallimakhos.Application.Interfaces;
using Kallimakhos.Application.Ports;
using Kallimakhos.Domain.Entities;
using Kallimakhos.Entities.Domain;

namespace Kallimakhos.Application.UseCases
{
    public class AddInfrastructureLayer : IAddInfrastructureLayer
    {
        public void Execute(SettingsInput settingsInput)
        {
            Console.WriteLine("Adding infrastructure layer...");

            // Add infrastructure folder to solution
            new Solution(
                    projectName: settingsInput.ProjectName,
                    projectPath: settingsInput.ProjectPath
                )
                .AddInfraLayer();

            // Add data project to solution
            DataProject? dataProjet = null;
            if (settingsInput.HasDatabase)
            {
                Console.WriteLine("Adding data project...");

                // Add data project to solution
                dataProjet = new DataProject(
                    projectName: settingsInput.ProjectName,
                    projectPath: settingsInput.ProjectPath
                );
                dataProjet.AddLayer();
            }

            // Add external services project to solution
            ExternalServiceProject? externalServicesProject = null;
            if (settingsInput.HasExternalServices)
            {
                Console.WriteLine("Adding external services project...");

                // Add external services project to solution
                externalServicesProject = new ExternalServiceProject(
                    projectName: settingsInput.ProjectName,
                    projectPath: settingsInput.ProjectPath
                );
                externalServicesProject.AddLayer();
            }

            // Add presentation project to solution
            PresentationProject? presentationProject = null;
            if (!string.IsNullOrWhiteSpace(settingsInput.TypeUI))
            {
                Console.WriteLine("Adding presentation project...");

                // Add presentation project to solution
                presentationProject = new PresentationProject(
                    projectName: settingsInput.ProjectName,
                    projectPath: settingsInput.ProjectPath,
                    typeUI: settingsInput.TypeUI
                );
                presentationProject.AddLayer();
            }

            // Add IoC project to solution
            new IoCProject(
                    projectName: settingsInput.ProjectName,
                    projectPath: settingsInput.ProjectPath
                )
                .AddLayer(
                    dataProject: dataProjet?.DataProjectPath,
                    externalServicesProject: externalServicesProject?.ExternalServicesProjectPath,
                    uiProject: presentationProject?.UIProject
                );
        }
    }
}