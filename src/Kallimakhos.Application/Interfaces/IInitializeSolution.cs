using Kallimakhos.Application.Ports;

namespace Kallimakhos.Application.Interfaces
{
    public interface IInitializeSolution
    {
        void Execute(SettingsInput settingsInput);
    }
}