using Kallimakhos.Application.Interfaces;
using Kallimakhos.Application.UseCases;

namespace Kallimakhos.IoC.Factories
{
    public class ApplicationFactories
    {
        public static IAddDomainLayer CreateAddDomainLayer() => new AddDomainLayer();
        public static IAddApplicationLayer CreateAddApplicationLayer() => new AddApplicationLayer();
        public static IAddInfrastructureLayer CreateAddInfrastructureLayer() => new AddInfrastructureLayer();
        public static IInitializeSolution CreateInitializeSolution() => new InitializeSolution();
    }
}