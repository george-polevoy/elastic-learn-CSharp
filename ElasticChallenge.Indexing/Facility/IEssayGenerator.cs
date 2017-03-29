using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticChallenge.Model;

namespace ElasticChallenge.Facility
{
    public interface IBoolshitEssayGenerator
    {
        Task<EssayGeneratorResponse> GenerateByTermsAsync(IEnumerable<string> terms);
    }
}