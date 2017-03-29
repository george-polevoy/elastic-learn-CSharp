using ElasticChallenge.Model;
using Nest;

namespace ElasticChallenge.Facility
{
    public class EssayElasticSetup
    {
        public const string IndexName = "essayssearch";
        private readonly ConnectionSettings _connectionSettings;

        public EssayElasticSetup()
        {
            _connectionSettings = new ConnectionSettings()
                .DefaultIndex(IndexName)
                .InferMappingFor<EssayDocument>(i => i
                    .TypeName("essay")
                    .IndexName(IndexName)
                );
        }

        public ElasticClient GetClient()
        {
            return new ElasticClient(_connectionSettings);
        }
    }
}