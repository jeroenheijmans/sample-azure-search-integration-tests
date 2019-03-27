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
            await fixture.SearchService.RecreateIndexAsync();
            var dto = new PersonDto { Email = $"{Guid.NewGuid()}@example.org" };
            await fixture.SearchService.IndexAsync(dto);

            WaitForIndexing(dto);

            var searchResult = await fixture.SearchService.SearchAsync(dto.Email);

            Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
        }

        [Fact]
        public async Task Can_find_person_by_exact_and_unique_lastname()
        {
            await fixture.SearchService.RecreateIndexAsync();
            var dto = new PersonDto { LastName = $"{Guid.NewGuid()}@example.org" };
            await fixture.SearchService.IndexAsync(dto);

            WaitForIndexing(dto);

            var searchResult = await fixture.SearchService.SearchAsync(dto.Email);

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
            await fixture.SearchService.RecreateIndexAsync();

            for (int i = 0; i < count; i++)
            {
                var guid = Guid.NewGuid().ToString().Replace("-", "");
                var dto = new PersonDto { Email = $"{guid}@example.org" };
                await fixture.SearchService.IndexAsync(dto);

                WaitForIndexing(dto);

                var searchResult = await fixture.SearchService.SearchAsync(dto.Id);

                Assert.Single(searchResult.Results, p => p.Document.Id == dto.Id);
            }
        }

        private void WaitForIndexing(PersonDto dto)
        {
            // If you use a *free* tier OR a tier with multiple replicas, it is not
            // reliable to spin until you can query the document, as you might get
            // a result from a replica here, when the real assertions and tests run
            // against a replica that is lagging behind.
            //
            // See also: https://stackoverflow.com/a/40117836/419956
            //
            // So instead, you should resort to something like this:
            //
            // Thread.Sleep(2000);

            var wait = 25;

            while (wait <= 2000)
            {
                Thread.Sleep(wait);
                var result = fixture.SearchService.FilterForId(dto.Id);
                if (result.Result.Results.Count == 1) return;
                if (result.Result.Results.Count > 1) throw new Exception("Unexpected results");
                wait *= 2;
            }

            throw new Exception("Found nothing after waiting a while");
        }
    }
}
