using System.Threading.Tasks;
using ElasticChallenge.Model;

namespace ElasticChallenge.Facility
{
    /// <summary>
    /// Indexes and searches documents.
    /// TODO: Consider splitting to separate interfaces for indexing and searching
    /// </summary>
    public interface IDocumentIndexer
    {
        /// <summary>
        /// Resets the index. Deletes existing essay index, if exists, and creates a new one.
        /// </summary>
        Task ReCreateIndexAsync();

        /// <summary>
        /// Appends document to index.
        /// </summary>        
        Task AppendDocumentAsync(EssayDocument request);

        /// <summary>
        /// Searches the essays in the index.
        /// </summary>
        Task<EssaySearchResponse> SearchEssays(EssaySearchRequest searchRequest);
    }
}