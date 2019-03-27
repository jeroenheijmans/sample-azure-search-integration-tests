using System;
using System.Linq;
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

        public async Task<DocumentIndexResult> IndexDocumentsInBatch(params PersonDto[] dtos)
        {
            return await _searchIndexClient.Documents.IndexAsync(IndexBatch.Upload(dtos));
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

                // To ensure stable results, we add a sensible default for
                // sort order. Later this might be moved to SearchQuery.
                OrderBy = new[] { "search.score() desc", "registeredAtUtc desc" },
            };

            return await _searchIndexClient.Documents.SearchAsync<PersonDto>(query, parameters);
        }
    }
}
