using BlogDoFT.Libs.Flagr;
using BlogDoFT.Libs.Keycloak;
using BlogDoFT.Libs.Flagr.Abstractions;
using FlagrExample.Providers;
using FlagrExample.Providers.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlagrExample
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("What feature is active?");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
                .AddUserSecrets(typeof(Program).Assembly, optional: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddKeycloakTokenProvider();

            services
                .AddFlagr()
                .AddHttpMessageHandler<KeycloakAuthenticationHandler>();

            services.AddSingleton<IFeatureProvider>(sp =>
                new FeatureProvider(sp.GetRequiredService<IFlagResolver>(), "app3"));

            using var serviceProvider = services.BuildServiceProvider();

            var featureProvider = serviceProvider.GetRequiredService<IFeatureProvider>();
            var feature = featureProvider.GetFeature();
            feature.Execute();
        }
    }
}
