using System.Collections.Generic;

namespace ElasticChallenge.Model
{
    public class EssaySearchRequest
    {
        public EssaySearchRequest(string query, IReadOnlyList<string> terms)
        {
            Query = query;
            Terms = terms;
        }

        public string Query { get; }
        public IReadOnlyList<string> Terms { get; }
    }
}
