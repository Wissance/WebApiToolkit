using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Configuration;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Core.Services;
using Wissance.WebApiToolkit.Ef.Extensions;
using Wissance.WebApiToolkit.Ef.Factories;
using Wissance.WebApiToolkit.Ef.Generators;
using Wissance.WebApiToolkit.TestApp.Data;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Dto;
using Wissance.WebApiToolkit.TestApp.Factories;
using Wissance.WebApiToolkit.TestApp.Managers;
using Wissance.WebApiToolkit.TestApp.WebServices.Grpc;

namespace Wissance.WebApiToolkit.TestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogging(services);
            ConfigureDatabase(services);
            ConfigureWebApi(services);
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(Configuration).AddConsole());
            services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(Configuration).AddDebug());
        }
        
        private void ConfigureDatabase(IServiceCollection services)
        {
            Guid id = Guid.NewGuid();
            services.AddDbContext<ModelContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseInMemoryDatabase(id.ToString()));
            // init database with test data
            ServiceProvider sp = services.BuildServiceProvider();
            ModelContext context = sp.GetRequiredService<ModelContext>();
            DataInitializer.Init(context);
        }

        private void ConfigureWebApi(IServiceCollection services)
        {
            ConfigureManagers(services);
            ConfigureControllers(services);
            services.AddGrpc();
            ConfigureWebServices(services);
        }

        private void ConfigureManagers(IServiceCollection services)
        {
            // 1. CodeManager && OrganizationManager were created manually
            services.AddScoped<CodeManager>(sp =>
            {
                // filter function was not written here yet
                return new CodeManager(sp.GetRequiredService<ModelContext>(),
                    null, CodeFactory.Create, sp.GetRequiredService<ILoggerFactory>());
            });

            services.AddScoped<OrganizationManager>(sp =>
            {
                return new OrganizationManager(sp.GetRequiredService<ModelContext>(),
                    null, OrganizationFactory.Create, sp.GetRequiredService<ILoggerFactory>());
            });
        }

        private void ConfigureWebServices(IServiceCollection services)
        {
            services.AddScoped<ResourceBasedDataManageableReadOnlyService<CodeDto, CodeEntity, int, EmptyAdditionalFilters>>(
                sp =>
                {
                    return new ResourceBasedDataManageableReadOnlyService<CodeDto, CodeEntity, int, EmptyAdditionalFilters>(sp.GetRequiredService<CodeManager>());
                });
        }

        private void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers();
            ServiceProvider provider = services.BuildServiceProvider();
            ManagerConfiguration<UserDto, UserEntity, int> userManagerConfig = new ManagerConfiguration<UserDto, UserEntity, int>()
            {
                CreateResFunc = UserFactory.Create,
                CreateObjFunc = UserFactory.Create,
                UpdateObjFunc = UserFactory.Update,
                FilterFunc = null
            };
            ManagerConfiguration<RoleDto, RoleEntity, int> roleManagerConfig = new ManagerConfiguration<RoleDto, RoleEntity, int>()
            {
                CreateResFunc = RoleFactory.Create,
                FilterFunc = null
            };
            Assembly userControllerAssembly = services.AddFullyConfiguredAutoController<UserDto, UserEntity, int, EmptyAdditionalFilters>(
                provider.GetRequiredService<ModelContext>(), "User",
                ControllerType.FullCrud, userManagerConfig, provider.GetRequiredService<ILoggerFactory>());
            Assembly roleControllerAssembly = services.AddFullyConfiguredAutoController<RoleDto, RoleEntity, int, EmptyAdditionalFilters>(
                provider.GetRequiredService<ModelContext>(), "Role",
                ControllerType.Bulk, roleManagerConfig, provider.GetRequiredService<ILoggerFactory>());
            
            services.AddControllers().AddApplicationPart(userControllerAssembly).AddControllersAsServices();
            services.AddControllers().AddApplicationPart(roleControllerAssembly).AddControllersAsServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<CodeGrpcService>();
            });
            
        }
        
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
    }
}