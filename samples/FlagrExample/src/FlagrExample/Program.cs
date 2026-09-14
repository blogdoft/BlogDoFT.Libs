using BlogDoFT.Libs.Flagr;
using BlogDoFT.Libs.Flagr.Abstractions;
using BlogDoFT.Libs.Keycloak;
using FlagrExample.Flags;
using FlagrExample.Providers;
using FlagrExample.Providers.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                new FeatureProvider(sp.GetRequiredService<IFlagEvaluator>(), "app3"));

            using var serviceProvider = services.BuildServiceProvider();

            ShowFeatureProviderStrategy(serviceProvider);
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine();

            ShowOnOffFlagStrategy(serviceProvider);
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine();

            ShowVariantAttachment(serviceProvider);
        }

        private static void ShowFeatureProviderStrategy(IServiceProvider serviceProvider)
        {
            var featureProvider = serviceProvider.GetRequiredService<IFeatureProvider>();
            var feature = featureProvider.GetFeature();
            feature.Execute();
        }

        private static void ShowOnOffFlagStrategy(IServiceProvider serviceProvider)
        {
            var flagrResolver = serviceProvider.GetRequiredService<IFlagEvaluator>();

            var response = flagrResolver.EvaluateFlag<OnOffFlag>();
            var flagState = response.VariantKey;

            var onOffFlag = new OnOffFlag(flagState);
            if (onOffFlag.IsOn)
            {
                Console.WriteLine("Feature is ON");
            }
            else
            {
                Console.WriteLine("Feature is OFF");
            }
        }

        private static void ShowVariantAttachment(IServiceProvider serviceProvider)
        {
            var flagrResolver = serviceProvider.GetRequiredService<IFlagEvaluator>();

            var response = flagrResolver.EvaluateFlag<VariantAttachmentFlag>();
            var variantAttachment = response.VariantAttachment.Deserialize<VariantAttachmentFlag>(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            });

            if (variantAttachment != null)
            {
                Console.WriteLine($"Variant Attachment: {variantAttachment.RemoteContent}");
            }
            else
            {
                Console.WriteLine("No Variant Attachment found.");
            }
        }
    }
}
