namespace TrashMob.Shared.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Extension methods for setting up mock repositories with test data.
    /// </summary>
    public static class MockRepositoryExtensions
    {
        /// <summary>
        /// Sets up the Get method to return a queryable from the provided data.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGet<T>(
            this Mock<IKeyedRepository<T>> mock,
            IEnumerable<T> data) where T : KeyedModel
        {
            var queryable = new TestAsyncEnumerable<T>(data);
            mock.Setup(r => r.Get()).Returns(queryable);
            return mock;
        }

        /// <summary>
        /// Sets up the Get method with expression filter to return filtered queryable.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGetWithFilter<T>(
            this Mock<IKeyedRepository<T>> mock,
            IEnumerable<T> data) where T : KeyedModel
        {
            mock.Setup(r => r.Get(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<bool>()))
                .Returns((Expression<Func<T, bool>> predicate, bool _) =>
                    new TestAsyncEnumerable<T>(data.AsQueryable().Where(predicate)));
            return mock;
        }

        /// <summary>
        /// Sets up GetAsync to return a specific entity by ID.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGetAsync<T>(
            this Mock<IKeyedRepository<T>> mock,
            T entity) where T : KeyedModel
        {
            mock.Setup(r => r.GetAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            return mock;
        }

        /// <summary>
        /// Sets up GetAsync to return entities from a collection by ID lookup.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGetAsync<T>(
            this Mock<IKeyedRepository<T>> mock,
            IEnumerable<T> data) where T : KeyedModel
        {
            mock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) =>
                    data.FirstOrDefault(e => e.Id == id));
            return mock;
        }

        /// <summary>
        /// Sets up GetWithNoTrackingAsync to return a specific entity by ID.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGetWithNoTrackingAsync<T>(
            this Mock<IKeyedRepository<T>> mock,
            T entity) where T : KeyedModel
        {
            mock.Setup(r => r.GetWithNoTrackingAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            return mock;
        }

        /// <summary>
        /// Sets up GetWithNoTrackingAsync to return entities from a collection by ID lookup.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupGetWithNoTrackingAsync<T>(
            this Mock<IKeyedRepository<T>> mock,
            IEnumerable<T> data) where T : KeyedModel
        {
            mock.Setup(r => r.GetWithNoTrackingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) =>
                    data.FirstOrDefault(e => e.Id == id));
            return mock;
        }

        /// <summary>
        /// Sets up AddAsync to return the entity that was added.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupAddAsync<T>(
            this Mock<IKeyedRepository<T>> mock) where T : KeyedModel
        {
            mock.Setup(r => r.AddAsync(It.IsAny<T>()))
                .ReturnsAsync((T entity) => entity);
            return mock;
        }

        /// <summary>
        /// Sets up UpdateAsync to return the entity that was updated.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupUpdateAsync<T>(
            this Mock<IKeyedRepository<T>> mock) where T : KeyedModel
        {
            mock.Setup(r => r.UpdateAsync(It.IsAny<T>()))
                .ReturnsAsync((T entity) => entity);
            return mock;
        }

        /// <summary>
        /// Sets up DeleteAsync to return 1 (success).
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupDeleteAsync<T>(
            this Mock<IKeyedRepository<T>> mock) where T : KeyedModel
        {
            mock.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(1);
            return mock;
        }

        /// <summary>
        /// Sets up all common repository methods with the provided data.
        /// </summary>
        public static Mock<IKeyedRepository<T>> SetupAll<T>(
            this Mock<IKeyedRepository<T>> mock,
            IEnumerable<T> data) where T : KeyedModel
        {
            return mock
                .SetupGet(data)
                .SetupGetWithFilter(data)
                .SetupGetAsync(data)
                .SetupGetWithNoTrackingAsync(data)
                .SetupAddAsync()
                .SetupUpdateAsync()
                .SetupDeleteAsync();
        }

        /// <summary>
        /// Sets up Get method for IBaseRepository to return a queryable from the provided data.
        /// </summary>
        public static Mock<IBaseRepository<T>> SetupGet<T>(
            this Mock<IBaseRepository<T>> mock,
            IEnumerable<T> data) where T : BaseModel
        {
            var queryable = new TestAsyncEnumerable<T>(data);
            mock.Setup(r => r.Get()).Returns(queryable);
            return mock;
        }

        /// <summary>
        /// Sets up Get method with expression filter for IBaseRepository.
        /// </summary>
        public static Mock<IBaseRepository<T>> SetupGetWithFilter<T>(
            this Mock<IBaseRepository<T>> mock,
            IEnumerable<T> data) where T : BaseModel
        {
            mock.Setup(r => r.Get(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<bool>()))
                .Returns((Expression<Func<T, bool>> predicate, bool _) =>
                    new TestAsyncEnumerable<T>(data.AsQueryable().Where(predicate)));
            return mock;
        }
    }

    /// <summary>
    /// An IQueryable implementation that supports async enumeration for testing.
    /// </summary>
    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    /// <summary>
    /// An async enumerator that wraps a synchronous enumerator for testing.
    /// </summary>
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }

    /// <summary>
    /// A query provider that supports async query execution for testing.
    /// </summary>
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: [typeof(Expression)])
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(this, [expression]);

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, [executionResult]);
        }
    }
}
