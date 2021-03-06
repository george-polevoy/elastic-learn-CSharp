﻿using System.Threading.Tasks;
using ElasticChallenge.Facility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElasticChallenge.Indexing
{
    class Program
    {
        /// <summary>
        /// Number of documents to generate for the index to be a representable sample.
        /// </summary>
        private const int RequiredCount = 2000;

        /// <summary>
        /// I don't dig the concept of building the index at app startup, so I provide this console app to create index.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var reIndexCommand = buildServiceProvider.GetService<ReIndexCommand>();

            reIndexCommand.RequiredCount = RequiredCount;
            Task.Run(() => reIndexCommand.RunAsync()).Wait();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(
                    new LoggerFactory()
                    .AddConsole(LogLevel.Information)
                    .AddDebug(LogLevel.Trace))
                .AddLogging()
                .AddSingleton<EssayElasticSetup>()
                .AddTransient<IDocumentIndexer, ElasticDocumentIndexer>()
                .AddTransient<IBoolshitEssayGenerator, YandexBoolshitBoolshitEssayGeneratorGateway>()
                .AddTransient<ReIndexCommand>();
        }
    }
}
