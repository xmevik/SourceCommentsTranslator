using System.Text;
using System.Text.Json;

namespace SourceCommentsTranslator.Translators
{
    /// <summary>
    /// Provides translation services using the LibreTranslate API.
    /// </summary>
    public class LibreTranslate : ITranslator
    {
        private readonly string SourceLanguage;
        private readonly string DestinationLanguage;
        private readonly string RequestUri;
        private readonly HttpClient Client;

        /// <summary>
        /// Initializes a new instance of the LibreTranslateService class.
        /// </summary>
        /// <param name="src">The source language code.</param>
        /// <param name="dest">The destination language code.</param>
        /// <param name="requestUri">The URI of the LibreTranslate API endpoint.</param>
        public LibreTranslate(string src, string dest, string requestUri)
        {
            SourceLanguage = src;
            DestinationLanguage = dest;
            Client = new HttpClient();
            RequestUri = requestUri;
        }

        /// <summary>
        /// Translates the given text from the source language to the destination language.
        /// </summary>
        /// <param name="text">The text to be translated.</param>
        /// <returns>
        /// The translated text if the translation was successful; otherwise, returns the original text.
        /// </returns>
        public string Translate(string text)
        {
            var requestData = new
            {
                q = text,
                source = SourceLanguage,
                target = DestinationLanguage
            };
            string json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = Client.PostAsync(RequestUri, content).Result.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            var responseData = JsonSerializer.Deserialize<ResponseData>(jsonResponse);
            return responseData?.TranslatedText ?? text;
        }

        public class ResponseData
        {
            public DetectedLanguage? DetectedLanguage { get; set; }
            public string? TranslatedText { get; set; }
        }

        public class DetectedLanguage
        {
            public double Confidence { get; set; }
            public string? Language { get; set; }
        }

        public void Dispose()
        {
            Client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
