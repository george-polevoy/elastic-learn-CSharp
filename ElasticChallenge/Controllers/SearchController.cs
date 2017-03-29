using System.Threading.Tasks;
using ElasticChallenge.Facility;
using ElasticChallenge.Model;
using Microsoft.AspNetCore.Mvc;

namespace ElasticChallenge.Controllers
{
    [Produces("application/json")]
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly IDocumentIndexer _indexer;

        public SearchController(IDocumentIndexer indexer)
        {
            _indexer = indexer;
        }

        [HttpGet]
        public async Task<EssaySearchResponse> Get(string query, [FromQuery] string[] themes)
        {
            var searchRequest = new EssaySearchRequest(query, themes);
            return await _indexer.SearchEssays(searchRequest);
        }
    }
}