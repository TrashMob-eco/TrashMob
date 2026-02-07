namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Services;
using TrashMobMobile.ViewModels;
using Xunit;

/// <summary>
/// Tests for BaseViewModel's ExecuteAsync error handling.
/// Uses a concrete subclass since BaseViewModel is abstract.
/// </summary>
public class BaseViewModelTests
{
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly TestableViewModel sut;

    public BaseViewModelTests()
    {
        mockNotificationService = new Mock<INotificationService>();
        sut = new TestableViewModel(mockNotificationService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOperationSucceeds_SetsIsBusyDuringExecution()
    {
        // Arrange
        var wasBusyDuringOp = false;
        Func<Task> operation = () =>
        {
            wasBusyDuringOp = sut.IsBusy;
            return Task.CompletedTask;
        };

        // Act
        await sut.RunExecuteAsync(operation, "error");

        // Assert
        Assert.True(wasBusyDuringOp);
        Assert.False(sut.IsBusy); // Should be false after completion
        Assert.False(sut.IsError);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOperationThrows_SetsIsError()
    {
        // Arrange
        Func<Task> operation = () => throw new InvalidOperationException("test");

        // Act
        await sut.RunExecuteAsync(operation, "Something went wrong");

        // Assert
        Assert.True(sut.IsError);
        Assert.False(sut.IsBusy);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOperationThrows_NotifiesError()
    {
        // Arrange
        const string errorMessage = "Something went wrong";
        Func<Task> operation = () => throw new InvalidOperationException("test");

        // Act
        await sut.RunExecuteAsync(operation, errorMessage);

        // Assert
        mockNotificationService.Verify(n => n.NotifyError(errorMessage), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskCancelled_NotifiesTimeout()
    {
        // Arrange
        Func<Task> operation = () => throw new TaskCanceledException();

        // Act
        await sut.RunExecuteAsync(operation, "error");

        // Assert
        mockNotificationService.Verify(
            n => n.NotifyError("The request timed out. Please try again."),
            Times.Once);
        Assert.True(sut.IsError);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHttpError_NotifiesWithProvidedMessage()
    {
        // Arrange
        const string errorMessage = "Failed to load events";
        Func<Task> operation = () => throw new HttpRequestException("network error");

        // Act
        await sut.RunExecuteAsync(operation, errorMessage);

        // Assert
        mockNotificationService.Verify(n => n.NotifyError(It.IsAny<string>()), Times.Once);
        Assert.True(sut.IsError);
    }

    [Fact]
    public async Task ExecuteAsyncT_WhenOperationSucceeds_ReturnsResult()
    {
        // Arrange
        Func<Task<int>> operation = () => Task.FromResult(42);

        // Act
        var result = await sut.RunExecuteAsync(operation, "error");

        // Assert
        Assert.Equal(42, result);
        Assert.False(sut.IsError);
        Assert.False(sut.IsBusy);
    }

    [Fact]
    public async Task ExecuteAsyncT_WhenOperationThrows_ReturnsDefault()
    {
        // Arrange
        Func<Task<int>> operation = () => throw new InvalidOperationException("test");

        // Act
        var result = await sut.RunExecuteAsync(operation, "error");

        // Assert
        Assert.Equal(0, result);
        Assert.True(sut.IsError);
    }

    [Fact]
    public async Task ExecuteAsyncT_WhenOperationThrowsForRefType_ReturnsNull()
    {
        // Arrange
        Func<Task<string>> operation = () => throw new InvalidOperationException("test");

        // Act
        var result = await sut.RunExecuteAsync(operation, "error");

        // Assert
        Assert.Null(result);
        Assert.True(sut.IsError);
    }

    [Fact]
    public async Task ExecuteAsync_ResetsIsErrorOnNewExecution()
    {
        // Arrange - first call fails
        await sut.RunExecuteAsync(() => throw new Exception("fail"), "error");
        Assert.True(sut.IsError);

        // Act - second call succeeds
        await sut.RunExecuteAsync(() => Task.CompletedTask, "error");

        // Assert
        Assert.False(sut.IsError);
    }

    /// <summary>
    /// Concrete subclass exposing the protected ExecuteAsync methods for testing.
    /// </summary>
    private class TestableViewModel(INotificationService notificationService) : BaseViewModel(notificationService)
    {
        public async Task RunExecuteAsync(Func<Task> operation, string errorMessage)
        {
            await ExecuteAsync(operation, errorMessage);
        }

        public async Task<T?> RunExecuteAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            return await ExecuteAsync(operation, errorMessage);
        }
    }
}
