namespace TrashMob.Shared.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class QueryableExtensionsTests
    {
        private static IQueryable<int> CreateTestQueryable(int count)
        {
            return new TestAsyncEnumerable<int>(Enumerable.Range(1, count));
        }

        [Fact]
        public async Task ToPagedAsync_ReturnsFirstPage_WithCorrectMetadata()
        {
            var query = CreateTestQueryable(50);
            var parameters = new QueryParameters { Page = 1, PageSize = 10 };

            var result = await query.ToPagedAsync(parameters, x => x);

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(1, result.Pagination.Page);
            Assert.Equal(10, result.Pagination.PageSize);
            Assert.Equal(50, result.Pagination.TotalCount);
            Assert.Equal(5, result.Pagination.TotalPages);
            Assert.True(result.Pagination.HasNext);
            Assert.False(result.Pagination.HasPrevious);
        }

        [Fact]
        public async Task ToPagedAsync_ReturnsLastPage_WithCorrectMetadata()
        {
            var query = CreateTestQueryable(50);
            var parameters = new QueryParameters { Page = 5, PageSize = 10 };

            var result = await query.ToPagedAsync(parameters, x => x);

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(5, result.Pagination.Page);
            Assert.False(result.Pagination.HasNext);
            Assert.True(result.Pagination.HasPrevious);
        }

        [Fact]
        public async Task ToPagedAsync_ReturnsEmptyResult_WhenQueryIsEmpty()
        {
            var query = CreateTestQueryable(0);
            var parameters = new QueryParameters { Page = 1, PageSize = 10 };

            var result = await query.ToPagedAsync(parameters, x => x);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.Pagination.TotalCount);
            Assert.Equal(0, result.Pagination.TotalPages);
            Assert.False(result.Pagination.HasNext);
            Assert.False(result.Pagination.HasPrevious);
        }

        [Fact]
        public async Task ToPagedAsync_ClampsPageSize_ToMaximum()
        {
            var query = CreateTestQueryable(200);
            var parameters = new QueryParameters { Page = 1, PageSize = 500 };

            var result = await query.ToPagedAsync(parameters, x => x);

            Assert.Equal(100, result.Items.Count);
            Assert.Equal(100, result.Pagination.PageSize);
        }

        [Fact]
        public async Task ToPagedAsync_AppliesMapper_ToItems()
        {
            var query = CreateTestQueryable(5);
            var parameters = new QueryParameters { Page = 1, PageSize = 10 };

            var result = await query.ToPagedAsync(parameters, x => x * 10);

            Assert.Equal([10, 20, 30, 40, 50], result.Items);
        }

        [Fact]
        public async Task ToPagedAsync_HandlesPartialLastPage()
        {
            var query = CreateTestQueryable(13);
            var parameters = new QueryParameters { Page = 2, PageSize = 10 };

            var result = await query.ToPagedAsync(parameters, x => x);

            Assert.Equal(3, result.Items.Count);
            Assert.Equal(13, result.Pagination.TotalCount);
            Assert.Equal(2, result.Pagination.TotalPages);
        }

        [Fact]
        public void ToPagedResponse_WrapsItemsWithMetadata()
        {
            IReadOnlyList<string> items = ["a", "b", "c"];

            var result = items.ToPagedResponse(totalCount: 30, page: 2, pageSize: 3);

            Assert.Equal(3, result.Items.Count);
            Assert.Equal(30, result.Pagination.TotalCount);
            Assert.Equal(2, result.Pagination.Page);
            Assert.Equal(3, result.Pagination.PageSize);
            Assert.Equal(10, result.Pagination.TotalPages);
            Assert.True(result.Pagination.HasNext);
            Assert.True(result.Pagination.HasPrevious);
        }
    }
}
