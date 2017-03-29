using System;
using System.Collections.Generic;

namespace ElasticChallenge.Model
{
    public class EssayDocument
    {
        public EssayDocument(Guid id, string title, string content, IReadOnlyList<string> terms)
        {
            Id = id;
            Title = title;
            Content = content;
            Terms = terms;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Content { get; }
        public IReadOnlyList<string> Terms { get; }
    }
}
