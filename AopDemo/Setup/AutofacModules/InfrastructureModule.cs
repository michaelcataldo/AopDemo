using AopDemo.Ordering.Domain;
using AopDemo.Ordering.Infrastructure.Repositories;
using Autofac;

namespace AopDemo.Ordering.Api.Setup.AutofacModules
{
    public class InfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();
        }
    }
}