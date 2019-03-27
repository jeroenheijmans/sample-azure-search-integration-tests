using System;
using System.Text.RegularExpressions;

namespace AzureSearchExample
{
    public class AzureSearchConfiguration
    {
        public string ApiKey { get; }
        public string SearchServiceName { get; }
        public string IndexName { get; }

        public AzureSearchConfiguration(string apiKey, string searchServiceName)
        {
            // TODO: Introduce Environmet suffix to distinguish parallel builds

            var suffix = $"-user-{GetAzureSearchProofSuffix(Environment.UserName)}";

            ApiKey = apiKey;
            SearchServiceName = searchServiceName;
            IndexName = $"items{suffix}";
        }

        private static string GetAzureSearchProofSuffix(string input)
        {
            // Azure allows lowercase ascii and dashes
            // Using https://stackoverflow.com/a/123340/419956 for stripping ascii characters
            return Regex.Replace(input.ToLowerInvariant(), @"[^\u0000-\u007F]+", string.Empty);
        }
    }
}
