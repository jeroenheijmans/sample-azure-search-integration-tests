using Microsoft.Extensions.Configuration;
using System;

namespace AzureSearchExample
{
    public class SearchTestFixture : IDisposable
    {
        private static readonly string searchServiceName;
        private static readonly string apiKey;

        static SearchTestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.dev.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            searchServiceName = configuration["searchServiceName"];
            apiKey = configuration["apiKey"];

            if (string.IsNullOrWhiteSpace(apiKey)) throw new Exception("Found no apiKey in appsettings or environment variables");
            if (string.IsNullOrWhiteSpace(searchServiceName)) throw new Exception("Found no searchServiceName in appsettings or environment variables");
        }

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
