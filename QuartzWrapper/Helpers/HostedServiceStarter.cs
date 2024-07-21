using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzWrapper.Helpers
{

    public class HostedServiceStarter<T> : IHostedService where T : class, IHostedService
    {
        private readonly T _concreteHostedService;

        public HostedServiceStarter(T concreteHostedService) // Will be injected via DI, then the hosted service will be of type HostedServiceStarter
        {
            _concreteHostedService = concreteHostedService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _concreteHostedService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _concreteHostedService.StopAsync(cancellationToken);
        }
    }

}
