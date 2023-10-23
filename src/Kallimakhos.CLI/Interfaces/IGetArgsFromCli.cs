using Kallimakhos.Application.Ports;

namespace Kallimakhos.CLI.Interfaces
{
    public interface IGetArgsFromCli
    {
        SettingsInput Execute(string[] args);
    }
}