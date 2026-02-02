namespace TrashMob.Shared.Managers
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Manages waiver PDF document generation and storage.
    /// </summary>
    public class WaiverDocumentManager : IWaiverDocumentManager
    {
        private const string WaiversContainerName = "waivers";
        private readonly ILogger<WaiverDocumentManager> logger;
        private readonly BlobServiceClient blobServiceClient;

        static WaiverDocumentManager()
        {
            // Configure QuestPDF for Community license (free for open source)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaiverDocumentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="blobServiceClient">The blob service client.</param>
        public WaiverDocumentManager(
            ILogger<WaiverDocumentManager> logger,
            BlobServiceClient blobServiceClient)
        {
            this.logger = logger;
            this.blobServiceClient = blobServiceClient;
        }

        /// <inheritdoc />
        public async Task<string> GenerateAndStoreWaiverPdfAsync(
            UserWaiver userWaiver,
            User user,
            CancellationToken cancellationToken = default)
        {
            var pdfBytes = GenerateWaiverPdf(userWaiver, user);

            var blobContainer = blobServiceClient.GetBlobContainerClient(WaiversContainerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var blobName = $"{userWaiver.UserId}/{userWaiver.Id}.pdf";
            var blobClient = blobContainer.GetBlobClient(blobName);

            using var stream = new MemoryStream(pdfBytes);
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "application/pdf" },
                cancellationToken: cancellationToken);

            logger.LogInformation(
                "Generated and stored waiver PDF for user {UserId}, waiver {UserWaiverId}",
                userWaiver.UserId,
                userWaiver.Id);

            return blobClient.Uri.ToString();
        }

        /// <inheritdoc />
        public byte[] GenerateWaiverPdf(UserWaiver userWaiver, User user)
        {
            var waiverVersion = userWaiver.WaiverVersion;
            var waiverText = userWaiver.WaiverTextSnapshot ?? waiverVersion?.WaiverText ?? string.Empty;

            // Strip HTML tags for plain text rendering
            var plainText = StripHtmlTags(waiverText);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(header => ComposeHeader(header, waiverVersion?.Name ?? "Waiver"));
                    page.Content().Element(content => ComposeContent(content, plainText, userWaiver, user));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        /// <inheritdoc />
        public async Task<Stream> GetWaiverPdfAsync(string documentUrl, CancellationToken cancellationToken = default)
        {
            // Extract blob name from URL
            var uri = new Uri(documentUrl);
            var blobName = uri.AbsolutePath.TrimStart('/');

            // Remove container name from path
            if (blobName.StartsWith(WaiversContainerName + "/", StringComparison.OrdinalIgnoreCase))
            {
                blobName = blobName.Substring(WaiversContainerName.Length + 1);
            }

            var blobContainer = blobServiceClient.GetBlobContainerClient(WaiversContainerName);
            var blobClient = blobContainer.GetBlobClient(blobName);

            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream, cancellationToken);
            stream.Position = 0;

            return stream;
        }

        private static void ComposeHeader(IContainer container, string waiverName)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("TrashMob.eco")
                        .FontSize(20)
                        .Bold()
                        .FontColor(Colors.Green.Darken3);

                    column.Item().Text(waiverName)
                        .FontSize(16)
                        .SemiBold();

                    column.Item().Text("Signed Waiver Document")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Medium);
                });
            });
        }

        private static void ComposeContent(
            IContainer container,
            string waiverText,
            UserWaiver userWaiver,
            User user)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(10);

                // Waiver text
                column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(textColumn =>
                {
                    textColumn.Item().Text("Waiver Text").Bold().FontSize(12);
                    textColumn.Item().PaddingTop(10).Text(waiverText).LineHeight(1.4f);
                });

                // Signature section
                column.Item().PaddingTop(20).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(sigColumn =>
                {
                    sigColumn.Item().Text("Electronic Signature").Bold().FontSize(12);
                    sigColumn.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Signee Name:").SemiBold();
                            col.Item().Text($"{user?.UserName ?? "N/A"}");
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Typed Legal Name:").SemiBold();
                            col.Item().Text($"{userWaiver.TypedLegalName}");
                        });
                    });
                });

                // Timestamp section
                column.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(timeColumn =>
                {
                    timeColumn.Item().Text("Acceptance Details").Bold().FontSize(12);
                    timeColumn.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Accepted Date (UTC):").SemiBold();
                            col.Item().Text($"{userWaiver.AcceptedDate:yyyy-MM-dd HH:mm:ss} UTC");
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Expiry Date:").SemiBold();
                            col.Item().Text($"{userWaiver.ExpiryDate:yyyy-MM-dd HH:mm:ss} UTC");
                        });
                    });
                    timeColumn.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Signing Method:").SemiBold();
                            col.Item().Text($"{userWaiver.SigningMethod ?? "Electronic"}");
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Document ID:").SemiBold();
                            col.Item().Text($"{userWaiver.Id}");
                        });
                    });
                });

                // Audit trail
                column.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(auditColumn =>
                {
                    auditColumn.Item().Text("Audit Trail").Bold().FontSize(12);
                    auditColumn.Item().PaddingTop(10).Text($"IP Address: {userWaiver.IPAddress ?? "Not recorded"}")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                    auditColumn.Item().Text($"User Agent: {TruncateUserAgent(userWaiver.UserAgent)}")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                // Minor section (if applicable)
                if (userWaiver.IsMinor)
                {
                    column.Item().PaddingTop(15).Border(1).BorderColor(Colors.Orange.Lighten2).Background(Colors.Orange.Lighten5).Padding(15).Column(minorColumn =>
                    {
                        minorColumn.Item().Text("Guardian Consent").Bold().FontSize(12);
                        minorColumn.Item().PaddingTop(10).Text($"Guardian Name: {userWaiver.GuardianName ?? "N/A"}");
                        minorColumn.Item().Text($"Relationship: {userWaiver.GuardianRelationship ?? "N/A"}");
                    });
                }
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(text =>
                    {
                        text.Span("Generated: ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.Span($"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    column.Item().Text("This document is an official record of waiver acceptance.")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
                row.ConstantItem(100).AlignRight().Text(text =>
                {
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" / ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        }

        private static string StripHtmlTags(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            // Remove HTML tags
            var noTags = Regex.Replace(html, "<[^>]*>", " ");

            // Decode common HTML entities
            noTags = noTags
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&#39;", "'");

            // Normalize whitespace
            noTags = Regex.Replace(noTags, @"\s+", " ").Trim();

            return noTags;
        }

        private static string TruncateUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return "Not recorded";
            }

            return userAgent.Length > 100 ? string.Concat(userAgent.AsSpan(0, 100), "...") : userAgent;
        }
    }
}
