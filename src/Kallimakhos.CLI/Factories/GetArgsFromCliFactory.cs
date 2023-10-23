using Kallimakhos.CLI.Interfaces;
using Kallimakhos.CLI.Services;

namespace Kallimakhos.CLI.Factories
{
    public static class GetArgsFromCliFactory
    {
        public static IGetArgsFromCli Create() => new GetArgsFromCli();
    }
}