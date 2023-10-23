using Kallimakhos.CLI.Interfaces;
using Kallimakhos.CLI.Services;

namespace Kallimakhos.CLI.Factories
{
    public static class GetArgsInteractivelyFactory
    {
        public static IGetArgsInteractively Create() => new GetArgsInteractively();
    }
}