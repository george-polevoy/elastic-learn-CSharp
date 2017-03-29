using System.Collections.Generic;

namespace ElasticChallenge.Model
{
    public class EssayLibraryMetadata
    {
        /// <summary>
        /// Metadata of terms, screened from HTML code of yandex referats service.
        /// <code>
        /// var source = File.ReadAllText(@"C:\sources\ElasticChallenge\ReverseEng\referats-screen-01.txt");
        ///	var termMatches =
        ///		Regex
        ///		.Matches(source,
        ///			@"for=""(?'theme'[a-z]+)""&gt;(?'displayName'\w+)&lt;")
        ///			.Cast&lt;Match&gt;()
        ///			.Select(m =&gt; new
        ///			{
        ///				m.Groups["theme"].Value,
        ///				DisplayName = m.Groups["displayName"].Value
        ///			});
        ///	var code = string.Join(Environment.NewLine,
        ///		termMatches.Select(tm =&gt; $@"new ThemeMetadata(""{tm.Value}"", ""{tm.DisplayName}""),"));</code>
        /// 
        /// Assigning some subjective probabilities of writing on a theme, so there would be any variability in aggregating counts.
        /// </summary>
        public static readonly IReadOnlyList<ThemeMetadata> Themes = new List<ThemeMetadata>
        {
            new ThemeMetadata("astronomy", "Астрономии", 0.1),
            new ThemeMetadata("geology", "Геологии", 0.2),
            new ThemeMetadata("gyroscope", "Гироскопии", 0.01),
            new ThemeMetadata("literature", "Литературоведению", 0.3),
            new ThemeMetadata("marketing", "Маркетингу", 0.8),
            new ThemeMetadata("mathematics", "Математике", 0.2),
            new ThemeMetadata("music", "Музыковедению", 0.05),
            new ThemeMetadata("polit", "Политологии", 0.4),
            new ThemeMetadata("agrobiologia", "Почвоведению", 0.3),
            new ThemeMetadata("law", "Правоведению", 0.45),
            new ThemeMetadata("psychology", "Психологии", 0.95),
            new ThemeMetadata("geography", "Страноведению", 0.2),
            new ThemeMetadata("physics", "Физике", 0.71),
            new ThemeMetadata("philosophy", "Философии", 0.9),
            new ThemeMetadata("chemistry", "Химии", 0.5),
            new ThemeMetadata("estetica", "Эстетике", 0.04),
        };
    }
}
