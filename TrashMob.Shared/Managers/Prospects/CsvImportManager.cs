namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Imports community prospects from CSV files with deduplication and scoring.
    /// </summary>
    public class CsvImportManager(
        IKeyedRepository<CommunityProspect> prospectRepository,
        IProspectScoringManager scoringManager,
        ILogger<CsvImportManager> logger)
        : ICsvImportManager
    {
        // Expected columns: Name, Type, City, Region, Country, Population, Website, ContactEmail, ContactName, ContactTitle
        private const int MinColumns = 10;

        /// <inheritdoc />
        public async Task<CsvImportResult> ImportProspectsAsync(Stream csvStream, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = new CsvImportResult();
            var lines = await ReadLinesAsync(csvStream);

            if (lines.Count < 2)
            {
                result.Errors.Add(new CsvImportError { RowNumber = 0, Message = "CSV file is empty or has no data rows." });
                return result;
            }

            // Skip header row
            var existingProspects = await prospectRepository.Get().ToListAsync(cancellationToken);

            for (var i = 1; i < lines.Count; i++)
            {
                result.TotalRowsProcessed++;
                var rowNumber = i + 1; // 1-based, accounting for header

                var fields = ParseCsvLine(lines[i]);

                if (fields.Count < MinColumns)
                {
                    result.ErrorCount++;
                    result.Errors.Add(new CsvImportError
                    {
                        RowNumber = rowNumber,
                        Message = $"Expected at least {MinColumns} columns but found {fields.Count}.",
                    });
                    continue;
                }

                var name = fields[0].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    result.ErrorCount++;
                    result.Errors.Add(new CsvImportError
                    {
                        RowNumber = rowNumber,
                        Message = "Name is required.",
                    });
                    continue;
                }

                var city = fields[2].Trim();
                var region = fields[3].Trim();

                // Check for duplicate (name + city + region, case-insensitive)
                var isDuplicate = existingProspects.Any(p =>
                    string.Equals(p.Name?.Trim(), name, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(p.City?.Trim(), city, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(p.Region?.Trim(), region, StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    result.SkippedDuplicateCount++;
                    continue;
                }

                int? population = null;
                if (!string.IsNullOrWhiteSpace(fields[5]) && int.TryParse(fields[5].Trim(), out var pop))
                {
                    population = pop;
                }

                var prospect = new CommunityProspect
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Type = fields[1].Trim(),
                    City = city,
                    Region = region,
                    Country = fields[4].Trim(),
                    Population = population,
                    Website = fields[6].Trim(),
                    ContactEmail = fields[7].Trim(),
                    ContactName = fields[8].Trim(),
                    ContactTitle = fields[9].Trim(),
                    PipelineStage = 0,
                    FitScore = 0,
                    CreatedByUserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastUpdatedByUserId = userId,
                    LastUpdatedDate = DateTimeOffset.UtcNow,
                };

                try
                {
                    var added = await prospectRepository.AddAsync(prospect);

                    // Calculate and persist FitScore
                    var breakdown = await scoringManager.CalculateFitScoreAsync(added.Id, cancellationToken);
                    if (breakdown is not null && breakdown.TotalScore != 0)
                    {
                        added.FitScore = breakdown.TotalScore;
                        added.LastUpdatedByUserId = userId;
                        added.LastUpdatedDate = DateTimeOffset.UtcNow;
                        await prospectRepository.UpdateAsync(added);
                    }

                    // Track the newly added prospect for subsequent dedup checks
                    existingProspects.Add(added);
                    result.CreatedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to import prospect on row {Row}", rowNumber);
                    result.ErrorCount++;
                    result.Errors.Add(new CsvImportError
                    {
                        RowNumber = rowNumber,
                        Message = $"Import failed: {ex.Message}",
                    });
                }
            }

            return result;
        }

        private static async Task<List<string>> ReadLinesAsync(Stream stream)
        {
            List<string> lines = [];
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } line)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        /// <summary>
        /// Simple quoted-field-aware CSV parser. Handles fields wrapped in double quotes
        /// that may contain commas or escaped quotes ("").
        /// </summary>
        public static List<string> ParseCsvLine(string line)
        {
            List<string> fields = [];
            var current = new System.Text.StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var ch = line[i];

                if (inQuotes)
                {
                    if (ch == '"')
                    {
                        // Check for escaped quote ""
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(ch);
                    }
                }
                else
                {
                    if (ch == '"')
                    {
                        inQuotes = true;
                    }
                    else if (ch == ',')
                    {
                        fields.Add(current.ToString());
                        current.Clear();
                    }
                    else
                    {
                        current.Append(ch);
                    }
                }
            }

            fields.Add(current.ToString());
            return fields;
        }
    }
}
