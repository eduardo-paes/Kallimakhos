using Kallimakhos.CLI.Interfaces;
using Kallimakhos.CLI.Services;

namespace Kallimakhos.CLI.Factories
{
    public static class DisplayHelpMenuFactory
    {
        public static IDisplayHelpMenu Create() => new DisplayHelpMenu();
    }
}