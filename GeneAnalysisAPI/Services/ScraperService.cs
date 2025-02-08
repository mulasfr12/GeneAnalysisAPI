using AngleSharp;


namespace GeneAnalysisAPI.Services
{
    public class ScraperService
    {
        private readonly HttpClient _httpClient;

        public ScraperService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> ScrapeGeneData(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var geneFileUrl = document.QuerySelector("a[href*='IlluminaHiSeq_pancan_normalized_gene']")?.GetAttribute("href");

            return geneFileUrl;
        }
    }
}
