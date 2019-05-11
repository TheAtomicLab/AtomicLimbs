using LightInject;
using Microsoft.Azure.WebJobs.Host;

namespace Limbs.Worker
{
    public class SimpleInjectorJobActivator : IJobActivator
    {
        private readonly IServiceContainer _container;

        public SimpleInjectorJobActivator(IServiceContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}
