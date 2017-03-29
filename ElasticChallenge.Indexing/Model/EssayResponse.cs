using System;
using System.Collections.Generic;

namespace ElasticChallenge.Model
{
    public class EssayGeneratorResponse
    {
        public EssayGeneratorResponse(string title, string rawBody)
        {
            Title = title;
            RawBody = rawBody;
        }

        public string Title { get; }
        public string RawBody { get; }
    }
    public class EssayBrief
    {
        public EssayBrief(Guid id, string title)
        {
            Id = id;
            Title = title;
        }

        public Guid Id { get; }
        public string Title { get; }
    }

    public class TermBucket
    {
        public TermBucket(string term, long count)
        {
            Term = term;
            Count = count;
        }

        public string Term { get; set; }
        public long Count { get; set; }
    }

    public class EssaySearchResponse
    {
        public EssaySearchResponse(IReadOnlyList<EssayBrief> essays, IReadOnlyList<TermBucket> themes)
        {
            Essays = essays;
            Themes = themes;
        }

        public IReadOnlyList<EssayBrief> Essays { get; private set; }
        public IReadOnlyList<TermBucket> Themes { get; private set; }
    }
}
