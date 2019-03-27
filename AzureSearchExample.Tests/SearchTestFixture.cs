using System;
using System.IO;

namespace AzureSearchExample
{
    public class SearchTestFixture : IDisposable
    {
        private static readonly string searchServiceName = "sample-azure-search";

        // File is in the .gitignore file
        private static readonly string apiKey = File.ReadAllText("ApiKey.txt").Trim();

        public SearchService SearchService { get; }

        public SearchTestFixture()
        {
            SearchService = new SearchService(new AzureSearchConfiguration(apiKey, searchServiceName));
        }

        public void Dispose()
        {
            SearchService?.DeleteIndex();
        }
    }
}
