using System;
using System.Threading;
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
            await fixture.SearchService.IndexAsync(dto);

            WaitForIndexing();

            var searchResult = await fixture.SearchService.Search(dto.Email);

            Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
        }

        [Fact]
        public async Task Can_find_person_by_exact_and_unique_lastname()
        {
            await fixture.SearchService.RecreateIndex();
            var dto = new PersonDto { LastName = $"{Guid.NewGuid()}@example.org" };
            await fixture.SearchService.IndexAsync(dto);

            WaitForIndexing();

            var searchResult = await fixture.SearchService.Search(dto.Email);

            Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Can_index_and_then_find_person_many_times_in_a_row(int count)
        {
            await fixture.SearchService.RecreateIndex();

            for (int i = 0; i < count; i++)
            {
                var guid = Guid.NewGuid().ToString().Replace("-", "");
                var dto = new PersonDto { Email = $"{guid}@example.org" };
                await fixture.SearchService.IndexAsync(dto);

                WaitForIndexing();

                var searchResult = await fixture.SearchService.Search(dto.Id);

                Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
            }
        }

        private void WaitForIndexing()
        {
            // There seems to be no good way to determine if a document was both
            // indexed *and* ready to be found through searching. So instead we
            // resort to this:
            
            Thread.Sleep(2000);
        }
    }
}
