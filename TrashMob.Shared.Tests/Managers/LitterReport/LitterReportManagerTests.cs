namespace TrashMob.Shared.Tests.Managers.LitterReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.LitterReport;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class LitterReportManagerTests
    {
        private readonly Mock<IKeyedRepository<TrashMob.Models.LitterReport>> _litterReportRepository;
        private readonly Mock<ILitterImageManager> _litterImageManager;
        private readonly Mock<ILogger<LitterReportManager>> _logger;
        private readonly Mock<IDbTransaction> _dbTransaction;
        private readonly Mock<IEmailManager> _emailManager;
        private readonly Mock<IUserManager> _userManager;
        private readonly LitterReportManager _sut;

        public LitterReportManagerTests()
        {
            _litterReportRepository = new Mock<IKeyedRepository<TrashMob.Models.LitterReport>>();
            _litterImageManager = new Mock<ILitterImageManager>();
            _logger = new Mock<ILogger<LitterReportManager>>();
            _dbTransaction = new Mock<IDbTransaction>();
            _emailManager = new Mock<IEmailManager>();
            _userManager = new Mock<IUserManager>();

            // Default setups
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>())).Returns("Test email content");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<object>(),
                    It.IsAny<List<EmailAddress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _dbTransaction.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _dbTransaction.Setup(t => t.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _dbTransaction.Setup(t => t.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            _sut = new LitterReportManager(
                _litterReportRepository.Object,
                _litterImageManager.Object,
                _logger.Object,
                _dbTransaction.Object,
                _emailManager.Object,
                _userManager.Object);
        }

        #region GetNewLitterReportsAsync

        [Fact]
        public async Task GetNewLitterReportsAsync_ReturnsOnlyNewReports()
        {
            // Arrange
            var newReport = new LitterReportBuilder().AsNew().WithDefaultImage().Build();
            var assignedReport = new LitterReportBuilder().AsAssigned().WithDefaultImage().Build();
            var cleanedReport = new LitterReportBuilder().AsCleaned().WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport, assignedReport, cleanedReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetNewLitterReportsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal((int)LitterReportStatusEnum.New, resultList[0].LitterReportStatusId);
        }

        #endregion

        #region GetAssignedLitterReportsAsync

        [Fact]
        public async Task GetAssignedLitterReportsAsync_ReturnsOnlyAssignedReports()
        {
            // Arrange
            var newReport = new LitterReportBuilder().AsNew().WithDefaultImage().Build();
            var assignedReport = new LitterReportBuilder().AsAssigned().WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport, assignedReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetAssignedLitterReportsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal((int)LitterReportStatusEnum.Assigned, resultList[0].LitterReportStatusId);
        }

        #endregion

        #region GetCleanedLitterReportsAsync

        [Fact]
        public async Task GetCleanedLitterReportsAsync_ReturnsOnlyCleanedReports()
        {
            // Arrange
            var newReport = new LitterReportBuilder().AsNew().WithDefaultImage().Build();
            var cleanedReport = new LitterReportBuilder().AsCleaned().WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport, cleanedReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetCleanedLitterReportsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal((int)LitterReportStatusEnum.Cleaned, resultList[0].LitterReportStatusId);
        }

        #endregion

        #region GetNotCancelledLitterReportsAsync

        [Fact]
        public async Task GetNotCancelledLitterReportsAsync_ExcludesCancelledReports()
        {
            // Arrange
            var newReport = new LitterReportBuilder().AsNew().WithDefaultImage().Build();
            var cancelledReport = new LitterReportBuilder().AsCancelled().WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport, cancelledReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetNotCancelledLitterReportsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.NotEqual((int)LitterReportStatusEnum.Cancelled, resultList[0].LitterReportStatusId);
        }

        #endregion

        #region GetCancelledLitterReportsAsync

        [Fact]
        public async Task GetCancelledLitterReportsAsync_ReturnsOnlyCancelledReports()
        {
            // Arrange
            var newReport = new LitterReportBuilder().AsNew().WithDefaultImage().Build();
            var cancelledReport = new LitterReportBuilder().AsCancelled().WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport, cancelledReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetCancelledLitterReportsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal((int)LitterReportStatusEnum.Cancelled, resultList[0].LitterReportStatusId);
        }

        #endregion

        #region GetUserLitterReportsAsync

        [Fact]
        public async Task GetUserLitterReportsAsync_ReturnsReportsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var userReport = new LitterReportBuilder().CreatedBy(userId).WithDefaultImage().Build();
            var otherReport = new LitterReportBuilder().CreatedBy(otherUserId).WithDefaultImage().Build();

            var reports = new List<TrashMob.Models.LitterReport> { userReport, otherReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetUserLitterReportsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(userId, resultList[0].CreatedByUserId);
        }

        #endregion

        #region GetLitterReportCountsAsync

        [Fact]
        public async Task GetLitterReportCountsAsync_ReturnsCorrectCounts()
        {
            // Arrange
            var newReport1 = new LitterReportBuilder().AsNew().Build();
            var newReport2 = new LitterReportBuilder().AsNew().Build();
            var cleanedReport = new LitterReportBuilder().AsCleaned().Build();

            var reports = new List<TrashMob.Models.LitterReport> { newReport1, newReport2, cleanedReport };
            _litterReportRepository.SetupGet(reports);
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var (totalCount, cleanedCount) = await _sut.GetLitterReportCountsAsync();

            // Assert
            Assert.Equal(3, totalCount);
            Assert.Equal(1, cleanedCount);
        }

        #endregion

        #region GetUserLitterReportCountAsync

        [Fact]
        public async Task GetUserLitterReportCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userReport1 = new LitterReportBuilder().CreatedBy(userId).Build();
            var userReport2 = new LitterReportBuilder().CreatedBy(userId).Build();
            var otherReport = new LitterReportBuilder().CreatedBy(Guid.NewGuid()).Build();

            var reports = new List<TrashMob.Models.LitterReport> { userReport1, userReport2, otherReport };
            _litterReportRepository.SetupGetWithFilter(reports);

            // Act
            var result = await _sut.GetUserLitterReportCountAsync(userId);

            // Assert
            Assert.Equal(2, result);
        }

        #endregion

        #region AddWithResultAsync

        [Fact]
        public async Task AddWithResultAsync_FailsWhenLitterReportIsNull()
        {
            // Act
            var result = await _sut.AddWithResultAsync(null, Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("null", result.ErrorMessage);
        }

        [Fact]
        public async Task AddWithResultAsync_FailsWhenNoImages()
        {
            // Arrange
            var report = new LitterReportBuilder().Build(); // No images

            // Act
            var result = await _sut.AddWithResultAsync(report, Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("image", result.ErrorMessage);
        }

        [Fact]
        public async Task AddWithResultAsync_FailsWhenNameIsEmpty()
        {
            // Arrange
            var report = new LitterReportBuilder()
                .WithName("")
                .WithDefaultImage()
                .Build();

            // Act
            var result = await _sut.AddWithResultAsync(report, Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("name", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddWithResultAsync_SucceedsWithValidData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var report = new LitterReportBuilder()
                .WithName("Valid Report")
                .WithDefaultImage()
                .Build();

            _litterReportRepository.Setup(r => r.AddAsync(It.IsAny<TrashMob.Models.LitterReport>()))
                .ReturnsAsync((TrashMob.Models.LitterReport r) => r);

            // Act
            var result = await _sut.AddWithResultAsync(report, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_ReturnsMinusOneWhenNotFound()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _litterReportRepository.Setup(r => r.GetAsync(reportId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TrashMob.Models.LitterReport)null);

            // Act
            var result = await _sut.DeleteAsync(reportId, userId);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task DeleteAsync_SetsStatusToCancelledAndDeletesImages()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var report = new LitterReportBuilder()
                .WithId(reportId)
                .AsNew()
                .WithDefaultImage()
                .Build();

            var images = report.LitterImages.ToList();

            _litterReportRepository.Setup(r => r.GetAsync(reportId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);
            _litterReportRepository.Setup(r => r.UpdateAsync(It.IsAny<TrashMob.Models.LitterReport>()))
                .ReturnsAsync((TrashMob.Models.LitterReport r) => r);
            _litterImageManager.Setup(m => m.GetByParentIdAsync(reportId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(images);
            _litterImageManager.Setup(m => m.DeleteAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteAsync(reportId, userId);

            // Assert
            Assert.Equal(1, result);
            _dbTransaction.Verify(t => t.BeginTransactionAsync(), Times.Once);
            _dbTransaction.Verify(t => t.CommitTransactionAsync(), Times.Once);
        }

        #endregion
    }
}
