using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElasticChallenge.Facility;
using ElasticChallenge.Model;
using Microsoft.Extensions.Logging;

namespace ElasticChallenge.Indexing
{
    public class ReIndexCommand
    {
        private readonly IBoolshitEssayGenerator _boolshitEssayGenerator;
        private readonly IDocumentIndexer _indexer;
        private readonly ILogger<ReIndexCommand> _logger;

        public ReIndexCommand(IBoolshitEssayGenerator boolshitEssayGenerator, IDocumentIndexer indexer, ILogger<ReIndexCommand> logger)
        {
            _boolshitEssayGenerator = boolshitEssayGenerator;
            _indexer = indexer;
            _logger = logger;
        }

        public int RequiredCount { get; set; }

        public async Task RunAsync()
        {
            _logger.LogInformation("Indexing started.");

            _logger.LogInformation("Insuring index");

            await _indexer.ReCreateIndexAsync();

            const int maxParallelRequests = 500;

            var noMoreThanFewParallelRequests = new SemaphoreSlim(maxParallelRequests, maxParallelRequests);

            var allTasks = new ConcurrentDictionary<Guid, Task>();

            foreach (var combination in GetRandomThemeCombinations().Take(RequiredCount))
            {
                var termList = combination.Select(i => i.IdentifierName).ToList();
                _logger.LogTrace($"Parallel op semaphore count: {noMoreThanFewParallelRequests.CurrentCount}");
                await noMoreThanFewParallelRequests.WaitAsync();
                var opId = Guid.NewGuid();
                var task = new Task(async () =>
                {
                    try
                    {
                        // Assuming we need an external id.
                        var externalDocId = Guid.NewGuid();
                        var generated = await _boolshitEssayGenerator.GenerateByTermsAsync(termList);
                        var indexRequest = new EssayDocument(externalDocId, generated.Title, generated.RawBody, termList);
                        await _indexer.AppendDocumentAsync(indexRequest);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error seeding essay index.", e);
                    }
                    finally
                    {
                        noMoreThanFewParallelRequests.Release();
                        allTasks.TryRemove(opId, out Task unused);
                    }
                });

                allTasks.TryAdd(opId, task);
                task.Start();
            }

            List<Task> leftTasks;
            while ((leftTasks = allTasks.Values.ToList()).Count > 0)
                await Task.WhenAll(leftTasks);
        }

        private static IEnumerable<List<ThemeMetadata>> GetRandomThemeCombinations()
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            var themesCount = EssayLibraryMetadata.Themes.Count;
            var singleThemeProbability = 1.0 / themesCount / EssayLibraryMetadata.Themes.Average(t => t.SimulatedProbabilityOfWriting);
            var combinationsQ =
                from i in Enumerable.Range(0, int.MaxValue)
                let combination = (
                    from themeIndex in Enumerable.Range(0, themesCount)
                    let theme = EssayLibraryMetadata.Themes[themeIndex]
                    where r.NextDouble() < singleThemeProbability * theme.SimulatedProbabilityOfWriting
                    select theme)
                .ToList()
                where combination.Count > 0
                select combination;
            return combinationsQ;
        }
    }
}