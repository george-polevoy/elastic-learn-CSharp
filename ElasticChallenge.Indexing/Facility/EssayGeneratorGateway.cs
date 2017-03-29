using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ElasticChallenge.Model;

namespace ElasticChallenge.Facility
{
    /// <summary>
    /// Generates
    /// </summary>
    public class YandexBoolshitBoolshitEssayGeneratorGateway : IBoolshitEssayGenerator
    {
        /// <summary>
        /// Pattern of yandex bullshit essay generator.
        /// Appending the list of terms joined with '+' should yield a valid gateway url, such as:
        /// https://yandex.ru/referats/write/?t=mathematics+physics+chemestry
        /// </summary>
        private const string EssayApiPatten = @"https://yandex.ru/referats/write/?t=";

        public async Task<EssayGeneratorResponse> GenerateByTermsAsync(IEnumerable<string> terms)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(GetApiUrl(terms));
                return Map(result);
            }
        }

        /// <summary>
        /// Parse and map raw gateway result to meaningful parts.
        /// Should be a method in Elastic's own 
        /// </summary>
        /// <remarks>
        /// HTML snippet is in following form:
        /// &lt;div&gt;Реферат по математике и физике&lt;/div&gt;&lt;strong&gt;Тема: «Положительный бозе-конденсат: основные моменты»&lt;/strong&gt;&lt;p&gt;Постулат охватывает...
        /// </remarks>
        /// <param name="rawResponseBody">Response from gateway in a form of HTML snippet.</param>
        private EssayGeneratorResponse Map(string rawResponseBody)
        {
            var titleStart = "<strong>Тема: «";
            var titleEnd = "»</strong>";

            var titleStartIndex = rawResponseBody.IndexOf(titleStart, StringComparison.Ordinal);
            var titleEndIndex = rawResponseBody.IndexOf(titleEnd, StringComparison.Ordinal);

            var title = rawResponseBody.Substring(titleStartIndex + titleStart.Length, titleEndIndex - titleStartIndex - titleStart.Length);
            var body = rawResponseBody.Substring(titleEndIndex + titleEnd.Length);

            return new EssayGeneratorResponse(title, body);
        }

        private string GetApiUrl(IEnumerable<string> terms)
        {
            return $"{EssayApiPatten}{string.Join("+", terms)}";
        }
        
    }
}

