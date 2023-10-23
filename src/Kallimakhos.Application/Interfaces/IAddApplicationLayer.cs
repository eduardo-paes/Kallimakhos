using Kallimakhos.Application.Ports;

namespace Kallimakhos.Application.Interfaces
{
    public interface IAddApplicationLayer
    {
        void Execute(SettingsInput settingsInput);
    }
}