using System.Text.Json;

namespace SourceCommentsTranslator.Translators
{
    /// <summary>
    /// Provides translation services using the Google Translate API.
    /// </summary>
    public class GoogleTranslator : ITranslator
    {
        private readonly string SourceLanguage;
        private readonly string DestinationLanguage;
        private const string RequestUri = "https://translate.googleapis.com/translate_a/single";
        private readonly HttpClient Client;

        /// <summary>
        /// Initializes a new instance of the GoogleTranslatorService class.
        /// </summary>
        /// <param name="src">The source language code.</param>
        /// <param name="dest">The destination language code.</param>
        public GoogleTranslator(string src, string dest)
        {
            SourceLanguage = src;
            DestinationLanguage = dest;
            Client = new HttpClient();
        }

        /// <summary>
        /// Translates the given text from the source language to the destination language.
        /// </summary>
        /// <param name="text">The text to be translated.</param>
        /// <returns>
        /// The translated text if successful; otherwise, returns the original text.
        /// </returns>
        public string Translate(string text)
        {
            var requestData = new
            {
                input = SourceLanguage,
                output = DestinationLanguage,
                text
            };
            string client = "gtx";
            string dt = "t";
            string dj = "1";
            string sl = requestData.input;
            string tl = requestData.output;
            string q = Uri.EscapeDataString(requestData.text);

            string requestUrl = $"{RequestUri}?client={client}&dt={dt}&dj={dj}&sl={sl}&tl={tl}&q={q}";

            HttpResponseMessage response = Client.GetAsync(requestUrl).Result.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            var responseData = JsonSerializer.Deserialize<ResponseData>(jsonResponse);
            return responseData?.Sentences?.Trans ?? text;
        }

        public class ResponseData
        {
            public TextParams? Sentences { get; set; }
            public string? Src { get; set; }
            public object? Spell { get; set; }
        }

        public class TextParams
        {
            public string? Trans { get; set; }
            public string? Prig { get; set; }
            public int? Backend { get; set; }
        }

        public void Dispose()
        {
            Client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
