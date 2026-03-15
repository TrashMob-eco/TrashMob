namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Linq;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using TrashMob.Models;

    /// <summary>
    /// Generates PDF participation reports for volunteers.
    /// </summary>
    public static class ParticipationReportPdfGenerator
    {
        /// <summary>
        /// Generates a PDF participation report as a byte array.
        /// </summary>
        public static byte[] Generate(
            Event evt,
            User volunteer,
            EventAttendeeMetrics metrics,
            string verifiedByName)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var effectiveBags = metrics.Status == "Adjusted" && metrics.AdjustedBagsCollected.HasValue
                ? metrics.AdjustedBagsCollected.Value
                : metrics.BagsCollected ?? 0;

            var effectiveWeight = metrics.Status == "Adjusted" && metrics.AdjustedPickedWeight.HasValue
                ? metrics.AdjustedPickedWeight.Value
                : metrics.PickedWeight ?? 0;

            var effectiveWeightUnitId = metrics.Status == "Adjusted" && metrics.AdjustedPickedWeightUnitId.HasValue
                ? metrics.AdjustedPickedWeightUnitId.Value
                : metrics.PickedWeightUnitId ?? 1;

            var effectiveDuration = metrics.Status == "Adjusted" && metrics.AdjustedDurationMinutes.HasValue
                ? metrics.AdjustedDurationMinutes.Value
                : metrics.DurationMinutes ?? 0;

            var weightUnit = effectiveWeightUnitId == 2 ? "kg" : "lbs";
            var hours = effectiveDuration / 60;
            var minutes = effectiveDuration % 60;
            var durationText = hours > 0
                ? $"{hours} hour{(hours != 1 ? "s" : "")}{(minutes > 0 ? $" {minutes} min" : "")}"
                : $"{minutes} minutes";

            var eventDate = evt.EventDate.ToLocalTime();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("TrashMob.eco")
                                .FontSize(28).Bold().FontColor("#005C5C");
                            row.ConstantItem(150).AlignRight().Text("Volunteer\nParticipation Report")
                                .FontSize(12).FontColor(Colors.Grey.Medium).AlignRight();
                        });

                        col.Item().PaddingTop(5).LineHorizontal(2).LineColor("#005C5C");
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Volunteer info
                        col.Item().PaddingBottom(15).Text(text =>
                        {
                            text.Span("This report certifies that ").FontSize(12);
                            text.Span(volunteer.DisplayFirstName)
                                .FontSize(12).Bold();
                            text.Span(" participated in the following community cleanup event organized through TrashMob.eco.")
                                .FontSize(12);
                        });

                        // Event details card
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(card =>
                        {
                            card.Item().Text("Event Details").FontSize(14).Bold().FontColor("#005C5C");
                            card.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(130);
                                    columns.RelativeColumn();
                                });

                                AddRow(table, "Event Name:", evt.Name);
                                AddRow(table, "Date:", eventDate.ToString("D"));
                                AddRow(table, "Time:", eventDate.ToString("t"));
                                AddRow(table, "Location:", FormatLocation(evt));
                            });
                        });

                        col.Item().PaddingTop(15);

                        // Volunteer contribution card
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(card =>
                        {
                            card.Item().Text("Volunteer Contribution").FontSize(14).Bold().FontColor("#005C5C");
                            card.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(130);
                                    columns.RelativeColumn();
                                });

                                AddRow(table, "Duration:", durationText);
                                AddRow(table, "Bags Collected:", effectiveBags.ToString());
                                AddRow(table, "Weight Picked Up:", $"{effectiveWeight:F1} {weightUnit}");
                            });
                        });

                        col.Item().PaddingTop(25);

                        // Verification
                        col.Item().Border(1).BorderColor("#005C5C").Background("#F0F9F9").Padding(15).Column(card =>
                        {
                            card.Item().Row(row =>
                            {
                                row.RelativeItem().Column(vcol =>
                                {
                                    vcol.Item().Text("Verified").FontSize(16).Bold().FontColor("#005C5C");
                                    vcol.Item().PaddingTop(5).Text($"Reviewed by: {verifiedByName}")
                                        .FontSize(11);
                                    vcol.Item().Text($"Review date: {metrics.ReviewedDate?.ToLocalTime().ToString("D") ?? eventDate.ToString("D")}")
                                        .FontSize(11);
                                });
                            });
                        });

                        col.Item().PaddingTop(20);

                        // Disclaimer
                        col.Item().Text("This document is a record of volunteer participation tracked through TrashMob.eco. " +
                                       "TrashMob.eco is a 501(c)(3) non-profit organization dedicated to community environmental cleanups.")
                            .FontSize(9).FontColor(Colors.Grey.Medium).Italic();
                    });

                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text($"Generated {DateTime.UtcNow:MMMM d, yyyy} — trashmob.eco")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                        row.ConstantItem(100).AlignRight().Text($"Report ID: {Guid.NewGuid().ToString()[..8].ToUpperInvariant()}")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().PaddingVertical(3).Text(label).Bold().FontSize(11);
            table.Cell().PaddingVertical(3).Text(value).FontSize(11);
        }

        private static string FormatLocation(Event evt)
        {
            var parts = new[] { evt.StreetAddress, evt.City, evt.Region, evt.PostalCode }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(", ", parts);
        }
    }
}
