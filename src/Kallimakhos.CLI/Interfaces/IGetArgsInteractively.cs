using Kallimakhos.Application.Ports;

namespace Kallimakhos.CLI.Interfaces
{
    public interface IGetArgsInteractively
    {
        SettingsInput Execute();
    }
}