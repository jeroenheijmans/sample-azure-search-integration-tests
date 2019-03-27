using System;
using System.Threading.Tasks;
using Xunit;

namespace AzureSearchExample
{
    public class SearchServiceTests : IClassFixture<SearchTestFixture>
    {
        private readonly SearchTestFixture fixture;

        public SearchServiceTests(SearchTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Can_find_person_by_exact_and_unique_email()
        {
            await fixture.SearchService.RecreateIndex();
            var dto = new PersonDto { Email = $"{Guid.NewGuid()}@example.org" };
            await fixture.SearchService.IndexDocumentsInBatch(dto);

            WaitForIndexing();

            var searchResult = await fixture.SearchService.Search(dto.Email);

            Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
        }

        [Fact]
        public async Task Can_find_person_by_exact_and_unique_lastname()
        {
            await fixture.SearchService.RecreateIndex();
            var dto = new PersonDto { LastName = $"{Guid.NewGuid()}@example.org" };
            await fixture.SearchService.IndexDocumentsInBatch(dto);

            WaitForIndexing();

            var searchResult = await fixture.SearchService.Search(dto.Email);

            Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
        }

        private static void WaitForIndexing()
        {
            // TODO: Improve this.
            System.Threading.Thread.Sleep(1000);
        }
    }
}
