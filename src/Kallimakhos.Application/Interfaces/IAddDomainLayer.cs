using Kallimakhos.Application.Ports;

namespace Kallimakhos.Application.Interfaces
{
    public interface IAddDomainLayer
    {
        void Execute(SettingsInput settingsInput);
    }
}