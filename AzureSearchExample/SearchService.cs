using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSearchExample
{
    public class SearchService
    {
        private static readonly string[] _highlightFields = 
            typeof(PersonDto)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(IsSearchableAttribute))) // [IsSearchable] attribute
                .Select(prop => char.ToLower(prop.Name[0]) + prop.Name.Substring(1)) // lowerCamelCase
                .ToArray();

        // To ensure stable results, we add a sensible default for
        // sort order. Later this might be moved to SearchQuery.
        private static readonly string[] _defaultOrder = new[] { "search.score() desc", "registeredAtUtc desc" };

        public AzureSearchConfiguration _configuration;
        private readonly ISearchServiceClient _searchAdminClient;
        private readonly ISearchIndexClient _searchIndexClient;

        public SearchService(AzureSearchConfiguration configuration)
        {
            _configuration = configuration;

            var azureCredentials = new SearchCredentials(_configuration.ApiKey);
            _searchAdminClient = new SearchServiceClient(_configuration.SearchServiceName, azureCredentials);
            _searchIndexClient = new SearchIndexClient(_configuration.SearchServiceName, _configuration.IndexName, azureCredentials);
        }

        public async Task DeleteIndexAsync()
        {
            await _searchAdminClient.Indexes.DeleteAsync(_configuration.IndexName);
        }

        public void DeleteIndex()
        {
            _searchAdminClient.Indexes.Delete(_configuration.IndexName);
        }

        public async Task RecreateIndex()
        {
            await DeleteIndexAsync();

            await _searchAdminClient.Indexes.CreateAsync(new Index
            {
                Name = _configuration.IndexName,
                Fields = FieldBuilder.BuildForType<PersonDto>(),
            });
        }

        public async Task<DocumentIndexResult> IndexAsync(params PersonDto[] dtos)
        {
            return await _searchIndexClient.Documents.IndexAsync(IndexBatch.Upload(dtos));
        }

        public long Count()
        {
            return _searchIndexClient.Documents.Count();
        }

        public async Task<DocumentSearchResult<PersonDto>> Search(string query)
        {
            var parameters = new SearchParameters
            {
                IncludeTotalResultCount = true,
                Top = 25,
                Skip = 0,
                Facets = new string[] { },
                HighlightFields = new string[] { },
                Filter = "",
                OrderBy = _defaultOrder,
            };

            return await _searchIndexClient.Documents.SearchAsync<PersonDto>(query, parameters);
        }

        public async Task<DocumentSearchResult<PersonDto>> FilterForId(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var _))
            {
                throw new ArgumentException("Can only filter for guid-like strings", nameof(id));
            }

            var parameters = new SearchParameters
            {
                Top = 2, // We expect only one, but return max 2 so we can double check for errors
                Skip = 0,
                Facets = new string[] { },
                HighlightFields = new string[] { },
                Filter = $"id eq '{id}'",
                OrderBy = _defaultOrder,
            };

            var result = await _searchIndexClient.Documents.SearchAsync<PersonDto>("*", parameters);

            if (result.Results.Count > 1)
            {
                throw new Exception($"Search filtering for id '{id}' unexpectedly returned more than 1 result. Are you sure you searched for an ID, and that it is unique?");
            }

            return result;
        }

        // Taken from https://lucene.apache.org/core/2_9_4/queryparsersyntax.html
        private static readonly ISet<char> specialChars = new HashSet<char>("+-&|!(){}[]^\"~*?:\\");

        private static string Sanitize(string input)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (!specialChars.Contains(input[i])) builder.Append(input[i]);
            }

            return builder.ToString();
        }
    }
}
