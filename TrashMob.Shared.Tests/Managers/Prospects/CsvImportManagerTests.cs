namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class CsvImportManagerTests
    {
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly Mock<IProspectScoringManager> _scoringManager;
        private readonly Mock<ILogger<CsvImportManager>> _logger;
        private readonly CsvImportManager _sut;

        private const string CsvHeader = "Name,Type,City,Region,Country,Population,Website,ContactEmail,ContactName,ContactTitle";

        public CsvImportManagerTests()
        {
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _scoringManager = new Mock<IProspectScoringManager>();
            _logger = new Mock<ILogger<CsvImportManager>>();

            _prospectRepo.SetupGet(new List<CommunityProspect>());
            _prospectRepo.SetupAddAsync();

            _scoringManager
                .Setup(s => s.CalculateFitScoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FitScoreBreakdown { TotalScore = 50 });

            _sut = new CsvImportManager(_prospectRepo.Object, _scoringManager.Object, _logger.Object);
        }

        [Fact]
        public async Task ImportProspects_ValidCsv_CreatesCorrectCount()
        {
            var csv = $"""
                {CsvHeader}
                Seattle,Municipality,Seattle,WA,United States,750000,https://seattle.gov,info@seattle.gov,John Doe,City Clerk
                Portland,Municipality,Portland,OR,United States,650000,https://portland.gov,info@portland.gov,Jane Smith,Mayor
                """;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(2, result.CreatedCount);
            Assert.Equal(0, result.ErrorCount);
            Assert.Equal(0, result.SkippedDuplicateCount);
            Assert.Equal(2, result.TotalRowsProcessed);
        }

        [Fact]
        public async Task ImportProspects_DuplicatesSkipped()
        {
            // Set up existing prospect in the repo
            var existing = new CommunityProspect
            {
                Id = Guid.NewGuid(),
                Name = "Seattle",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            _prospectRepo.SetupGet(new List<CommunityProspect> { existing });

            var csv = $"""
                {CsvHeader}
                Seattle,Municipality,Seattle,WA,United States,750000,https://seattle.gov,info@seattle.gov,John Doe,City Clerk
                Portland,Municipality,Portland,OR,United States,650000,https://portland.gov,info@portland.gov,Jane Smith,Mayor
                """;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(1, result.CreatedCount);
            Assert.Equal(1, result.SkippedDuplicateCount);
        }

        [Fact]
        public async Task ImportProspects_MissingName_ReturnsError()
        {
            var csv = $"""
                {CsvHeader}
                ,Municipality,Seattle,WA,United States,750000,https://seattle.gov,info@seattle.gov,John Doe,City Clerk
                """;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(0, result.CreatedCount);
            Assert.Equal(1, result.ErrorCount);
            Assert.Single(result.Errors);
            Assert.Contains("Name is required", result.Errors[0].Message);
        }

        [Fact]
        public async Task ImportProspects_InsufficientColumns_ReturnsError()
        {
            var csv = $"""
                {CsvHeader}
                Seattle,Municipality,Seattle
                """;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(0, result.CreatedCount);
            Assert.Equal(1, result.ErrorCount);
            Assert.Single(result.Errors);
            Assert.Contains("Expected at least 10 columns", result.Errors[0].Message);
        }

        [Fact]
        public async Task ImportProspects_EmptyFile_ReturnsError()
        {
            var csv = "";

            var result = await ImportCsvAsync(csv);

            Assert.Equal(0, result.CreatedCount);
            Assert.Single(result.Errors);
            Assert.Contains("empty", result.Errors[0].Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ImportProspects_HeaderOnly_ReturnsZeroCounts()
        {
            var csv = CsvHeader;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(0, result.CreatedCount);
            Assert.Equal(0, result.ErrorCount);
            Assert.Equal(0, result.TotalRowsProcessed);
        }

        [Fact]
        public async Task ImportProspects_QuotedFieldsWithCommas_ParsesCorrectly()
        {
            var csv = $"""
                {CsvHeader}
                "City of Portland, Oregon",Municipality,Portland,OR,United States,650000,https://portland.gov,info@portland.gov,"Smith, Jane",Mayor
                """;

            var result = await ImportCsvAsync(csv);

            Assert.Equal(1, result.CreatedCount);
            Assert.Equal(0, result.ErrorCount);
        }

        [Fact]
        public void ParseCsvLine_QuotedFieldsWithEscapedQuotes_HandledCorrectly()
        {
            var line = "\"Say \"\"hello\"\"\",field2,field3";
            var fields = CsvImportManager.ParseCsvLine(line);

            Assert.Equal(3, fields.Count);
            Assert.Equal("Say \"hello\"", fields[0]);
        }

        [Fact]
        public void ParseCsvLine_SimpleFields_SplitsCorrectly()
        {
            var line = "a,b,c,d";
            var fields = CsvImportManager.ParseCsvLine(line);

            Assert.Equal(4, fields.Count);
            Assert.Equal("a", fields[0]);
            Assert.Equal("d", fields[3]);
        }

        private async Task<CsvImportResult> ImportCsvAsync(string csvContent)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            return await _sut.ImportProspectsAsync(stream, Guid.NewGuid());
        }
    }
}
