namespace ElasticChallenge.Model
{
    public class ThemeMetadata
    {
        public ThemeMetadata(string identifierName, string displayName, double probabilityOfWriting)
        {
            SimulatedProbabilityOfWriting = probabilityOfWriting;
            IdentifierName = identifierName;
            DisplayName = displayName;
        }

        public string IdentifierName { get; private set; }
        public string DisplayName { get; private set; }
        public double SimulatedProbabilityOfWriting { get; private set; }
    }
}