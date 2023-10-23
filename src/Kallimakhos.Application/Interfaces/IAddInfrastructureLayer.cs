using Kallimakhos.Application.Ports;

namespace Kallimakhos.Application.Interfaces
{
    public interface IAddInfrastructureLayer
    {
        void Execute(SettingsInput settingsInput);
    }
}