using System;
using System.Reflection;
using AopDemo.Ordering.Api.Setup.AutofacModules;
using AopDemo.Ordering.Api.Setup.Filters;
using AopDemo.Ordering.Application.Behaviors;
using AopDemo.Ordering.Infrastructure;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AopDemo.Ordering.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add<HttpGlobalExceptionFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingContext>(options =>
                    {
                        options.UseSqlServer(Configuration["ConnectionStrings:OrderingContext"],
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(OrderingContext).GetTypeInfo().Assembly.GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    },
                    ServiceLifetime.Scoped
                );

            services.AddCustomConfiguration();

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.AddMediatR(typeof(LoggingBehavior<,>).Assembly);

            builder
                .RegisterModule<ApplicationModule>()
                .RegisterModule<InfrastructureModule>();

            return new AutofacServiceProvider(builder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var model = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(model)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }
    }
}