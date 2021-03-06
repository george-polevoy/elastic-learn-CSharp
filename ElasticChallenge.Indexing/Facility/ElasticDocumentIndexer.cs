﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ElasticChallenge.Model;
using Microsoft.Extensions.Logging;
using Nest;

namespace ElasticChallenge.Facility
{
    public class ElasticDocumentIndexer : IDocumentIndexer
    {
        private const string MatchedTermsAggregateIdentifier = "matched_terms";
        private const string WithStopwordsAnalyzerIdentifier = "analyze_with_stopwords";
        private const string ForAllWordsAnalyzerIdentifier = "analyze_all";
        private readonly ILogger<ElasticDocumentIndexer> _logger;
        private readonly EssayElasticSetup _setup;

        public ElasticDocumentIndexer(ILogger<ElasticDocumentIndexer> logger, EssayElasticSetup setup)
        {
            _logger = logger;
            _setup = setup;
        }

        public async Task ReCreateIndexAsync()
        {
            var client = _setup.GetClient();
            await RemoveIndexIfNeeded(client);
            await CreateIndex(client);
        }

        private async Task RemoveIndexIfNeeded(IElasticClient client)
        {
            if ((await client.IndexExistsAsync(_setup.IndexName)).Exists)
                await client.DeleteIndexAsync(_setup.IndexName);
        }

        private async Task CreateIndex(IElasticClient client)
        {
            var indexAsync = await client.CreateIndexAsync(_setup.IndexName, i => i
                .Mappings(m => m.Map<EssayDocument>(MapEssay))
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Snowball(WithStopwordsAnalyzerIdentifier, sd => sd.Language(SnowballLanguage.Russian)
                                .StopWords("как", "почему", "зачем", "очевидно")
                            )
                            .Snowball(ForAllWordsAnalyzerIdentifier, sd => sd.Language(SnowballLanguage.Russian))
                        )
                        .CharFilters(cf => cf.HtmlStrip("html"))
                    )));

            if (!indexAsync.IsValid)
                _logger.LogError("Index creation operation is reported as invalid.");
        }

        private static TypeMappingDescriptor<EssayDocument> MapEssay(TypeMappingDescriptor<EssayDocument> map)
        {
            return map
                .Properties(properties =>
                    properties
                        .Text(d => d.Name(ed => ed.Title).Analyzer(ForAllWordsAnalyzerIdentifier))
                        .Text(d => d.Name(ed => ed.Content).Analyzer(WithStopwordsAnalyzerIdentifier))
                        .Keyword(kp => kp.Name(ed => ed.Terms)));
        }

        public async Task AppendDocumentAsync(EssayDocument request)
        {
            _logger.LogInformation("appending document: " + new { TermCount = request.Terms.Count });

            var client = _setup.GetClient();

            var result = await client.IndexAsync(request);

            if (!result.IsValid)
                _logger.LogError("Document indexing operation is reported as invalid.");
        }

        /// <summary>
        /// Searches essays using terms aggregate.
        /// 
        /// <remarks>
        /// Size parameter is used with an exact value to eliminate accuracy error arizing from shard recombination.
        /// https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-terms-aggregation.html#_size
        /// </remarks>
        /// </summary>
        public async Task<EssaySearchResponse> SearchEssays(EssaySearchRequest searchRequest)
        {
            var client = _setup.GetClient();

            var result = await client.SearchAsync<EssayDocument>(
                sd => sd
                    .Query(q => MakeQuery(searchRequest, q))
                    .Source(MakeSource)
                    .Aggregations(MakeAggregations));

            var facets = result.Aggs.Terms<string>(MatchedTermsAggregateIdentifier);

            var essaySearchResponse = new EssaySearchResponse(
                result.Hits.Select(x => new EssayBrief(Guid.Parse(x.Id), x.Source.Title)).ToList(),
                facets.Buckets.Select(kb => new TermBucket(kb.Key, kb.DocCount.GetValueOrDefault())).ToList());

            return essaySearchResponse;
        }

        private static AggregationContainerDescriptor<EssayDocument> MakeAggregations(AggregationContainerDescriptor<EssayDocument> acd)
        {
            return acd
                .Terms(
                    MatchedTermsAggregateIdentifier,
                    t => t
                        .Field(x => x.Terms)
                        .Order(TermsOrder.CountDescending)
                        .Size(EssayLibraryMetadata.Themes.Count));
        }

        private static SourceFilterDescriptor<EssayDocument> MakeSource(SourceFilterDescriptor<EssayDocument> src)
        {
            return src.Includes(fd => fd.Field(ed => ed.Title));
        }

        private static QueryContainer MakeQuery(EssaySearchRequest searchRequest, QueryContainerDescriptor<EssayDocument> q)
        {
            return q.Bool(b => b
                .Filter(f => f
                    .Terms(t => t
                        .Name("termsToMatch")
                        .Field(ed => ed.Terms)
                        .Terms(searchRequest.Terms)))
                .Must(m => m
                    .MultiMatch(dm => dm
                        .Query(searchRequest.Query)
                        .Fields(fld => fld
                            .Field(ed => ed.Title, 2)
                            .Field(ed => ed.Content))
                        .TieBreaker(0.3))));
        }
    }
}