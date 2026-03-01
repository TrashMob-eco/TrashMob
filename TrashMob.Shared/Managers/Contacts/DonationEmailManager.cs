namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages sending donation-related emails (thank-you, receipts, appeals) with auto-logging as ContactNotes.
    /// </summary>
    public class DonationEmailManager(
        IDonationManager donationManager,
        IKeyedManager<Contact> contactManager,
        IContactNoteManager contactNoteManager,
        IEmailManager emailManager,
        ILogger<DonationEmailManager> logger) : IDonationEmailManager
    {
        /// <inheritdoc />
        public async Task SendThankYouAsync(Guid donationId, Guid userId, CancellationToken cancellationToken = default)
        {
            var donation = await donationManager.GetAsync(donationId, cancellationToken);
            if (donation == null) throw new InvalidOperationException($"Donation {donationId} not found.");

            var contact = await contactManager.GetAsync(donation.ContactId, cancellationToken);
            if (contact == null) throw new InvalidOperationException($"Contact {donation.ContactId} not found.");

            if (string.IsNullOrWhiteSpace(contact.Email))
                throw new InvalidOperationException("Contact does not have an email address.");

            var template = emailManager.GetHtmlEmailCopy(nameof(NotificationTypeEnum.DonationThankYou));
            var emailCopy = template
                .Replace("{ContactName}", GetContactDisplayName(contact))
                .Replace("{Amount}", donation.Amount.ToString("N2"))
                .Replace("{DonationDate}", donation.DonationDate.ToString("MMMM d, yyyy"))
                .Replace("{DonationType}", ((DonationTypeEnum)donation.DonationType).ToString())
                .Replace("{Campaign}", donation.Campaign ?? "General");

            var dynamicTemplateData = new
            {
                subject = "Thank You for Your Donation to TrashMob.eco!",
                emailCopy,
            };

            var recipients = new List<EmailAddress>
            {
                new() { Email = contact.Email, Name = GetContactDisplayName(contact) },
            };

            await emailManager.SendTemplatedEmailAsync(
                "Thank You for Your Donation to TrashMob.eco!",
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            // Log as ContactNote
            await CreateContactNoteAsync(
                contact.Id,
                (int)ContactNoteTypeEnum.ThankYou,
                "Thank You Sent",
                $"Thank-you email sent for ${donation.Amount:N2} donation on {donation.DonationDate:d}.",
                userId,
                cancellationToken);

            // Update donation flag
            donation.ThankYouSent = true;
            await donationManager.UpdateAsync(donation, userId, cancellationToken);

            logger.LogInformation("Sent thank-you email for donation {DonationId} to {Email}", donationId, contact.Email);
        }

        /// <inheritdoc />
        public async Task SendReceiptAsync(Guid donationId, Guid userId, CancellationToken cancellationToken = default)
        {
            var donation = await donationManager.GetAsync(donationId, cancellationToken);
            if (donation == null) throw new InvalidOperationException($"Donation {donationId} not found.");

            var contact = await contactManager.GetAsync(donation.ContactId, cancellationToken);
            if (contact == null) throw new InvalidOperationException($"Contact {donation.ContactId} not found.");

            if (string.IsNullOrWhiteSpace(contact.Email))
                throw new InvalidOperationException("Contact does not have an email address.");

            var inKindRow = donation.DonationType == (int)DonationTypeEnum.InKind && !string.IsNullOrWhiteSpace(donation.InKindDescription)
                ? $"<tr><td style=\"padding: 4px 16px 4px 0; font-weight: bold;\">Description:</td><td style=\"padding: 4px 0;\">{donation.InKindDescription}</td></tr>"
                : string.Empty;

            var template = emailManager.GetHtmlEmailCopy(nameof(NotificationTypeEnum.DonationReceipt));
            var emailCopy = template
                .Replace("{ContactName}", GetContactDisplayName(contact))
                .Replace("{Amount}", donation.Amount.ToString("N2"))
                .Replace("{DonationDate}", donation.DonationDate.ToString("MMMM d, yyyy"))
                .Replace("{DonationType}", ((DonationTypeEnum)donation.DonationType).ToString())
                .Replace("{InKindRow}", inKindRow)
                .Replace("{ReceiptDate}", DateTimeOffset.UtcNow.ToString("MMMM d, yyyy"));

            // Generate PDF receipt
            var pdfBytes = GenerateReceiptPdf(donation, contact);
            var base64Pdf = Convert.ToBase64String(pdfBytes);

            var dynamicTemplateData = new
            {
                subject = "Your Donation Receipt from TrashMob.eco",
                emailCopy,
            };

            var recipients = new List<EmailAddress>
            {
                new() { Email = contact.Email, Name = GetContactDisplayName(contact) },
            };

            var attachments = new List<EmailAttachment>
            {
                new()
                {
                    Filename = $"TrashMob_Receipt_{donation.DonationDate:yyyy-MM-dd}.pdf",
                    Base64Content = base64Pdf,
                    MimeType = "application/pdf",
                },
            };

            await emailManager.SendTemplatedEmailAsync(
                "Your Donation Receipt from TrashMob.eco",
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                attachments,
                cancellationToken);

            // Log as ContactNote
            await CreateContactNoteAsync(
                contact.Id,
                (int)ContactNoteTypeEnum.Email,
                "Tax Receipt Sent",
                $"Tax receipt email sent for ${donation.Amount:N2} donation on {donation.DonationDate:d}.",
                userId,
                cancellationToken);

            // Update donation flag
            donation.ReceiptSent = true;
            await donationManager.UpdateAsync(donation, userId, cancellationToken);

            logger.LogInformation("Sent receipt email for donation {DonationId} to {Email}", donationId, contact.Email);
        }

        /// <inheritdoc />
        public async Task SendAppealAsync(Guid contactId, string subject, string body, Guid userId, CancellationToken cancellationToken = default)
        {
            var contact = await contactManager.GetAsync(contactId, cancellationToken);
            if (contact == null) throw new InvalidOperationException($"Contact {contactId} not found.");

            if (string.IsNullOrWhiteSpace(contact.Email))
                throw new InvalidOperationException("Contact does not have an email address.");

            var template = emailManager.GetHtmlEmailCopy(nameof(NotificationTypeEnum.FundraisingAppeal));
            var emailCopy = template
                .Replace("{ContactName}", GetContactDisplayName(contact))
                .Replace("{Message}", body);

            var dynamicTemplateData = new
            {
                subject,
                emailCopy,
            };

            var recipients = new List<EmailAddress>
            {
                new() { Email = contact.Email, Name = GetContactDisplayName(contact) },
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            // Log as ContactNote
            await CreateContactNoteAsync(
                contact.Id,
                (int)ContactNoteTypeEnum.Appeal,
                subject,
                body,
                userId,
                cancellationToken);

            logger.LogInformation("Sent appeal email to contact {ContactId} at {Email}", contactId, contact.Email);
        }

        /// <inheritdoc />
        public async Task<BulkAppealResult> SendBulkAppealAsync(IEnumerable<Guid> contactIds, string subject, string body, Guid userId, CancellationToken cancellationToken = default)
        {
            var result = new BulkAppealResult();

            foreach (var contactId in contactIds)
            {
                try
                {
                    var contact = await contactManager.GetAsync(contactId, cancellationToken);
                    if (contact == null || string.IsNullOrWhiteSpace(contact.Email))
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    await SendAppealAsync(contactId, subject, body, userId, cancellationToken);
                    result.SentCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send appeal to contact {ContactId}", contactId);
                    result.FailedCount++;
                }
            }

            return result;
        }

        private byte[] GenerateReceiptPdf(Donation donation, Contact contact)
        {
            var donationType = ((DonationTypeEnum)donation.DonationType).ToString();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("TrashMob.eco")
                            .FontSize(20).Bold().FontColor(Colors.Green.Darken3);
                        column.Item().Text("Donation Receipt")
                            .FontSize(16).SemiBold();
                        column.Item().Text("Tax-Exempt Organization — EIN: 88-3607538")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(10);

                        // Donor info
                        column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(donorCol =>
                        {
                            donorCol.Item().Text("Donor Information").Bold().FontSize(12);
                            donorCol.Item().PaddingTop(10).Text($"Name: {GetContactDisplayName(contact)}");
                            if (!string.IsNullOrWhiteSpace(contact.OrganizationName))
                                donorCol.Item().Text($"Organization: {contact.OrganizationName}");
                            if (!string.IsNullOrWhiteSpace(contact.Address))
                                donorCol.Item().Text($"Address: {contact.Address}, {contact.City}, {contact.Region} {contact.PostalCode}");
                        });

                        // Donation details
                        column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(detailCol =>
                        {
                            detailCol.Item().Text("Donation Details").Bold().FontSize(12);
                            detailCol.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Date:").SemiBold();
                                    col.Item().Text($"{donation.DonationDate:MMMM d, yyyy}");
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Amount:").SemiBold();
                                    col.Item().Text($"${donation.Amount:N2}");
                                });
                            });
                            detailCol.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Type:").SemiBold();
                                    col.Item().Text(donationType);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Campaign:").SemiBold();
                                    col.Item().Text(donation.Campaign ?? "General");
                                });
                            });
                            if (donation.DonationType == (int)DonationTypeEnum.InKind && !string.IsNullOrWhiteSpace(donation.InKindDescription))
                            {
                                detailCol.Item().PaddingTop(5).Text($"In-Kind Description: {donation.InKindDescription}");
                            }
                        });

                        // IRS disclosure
                        column.Item().PaddingTop(20).Text(
                            "No goods or services were provided in exchange for this contribution. " +
                            "TrashMob.eco is a 501(c)(3) tax-exempt organization. Your donation is tax-deductible " +
                            "to the extent allowed by law. Please retain this receipt for your records.")
                            .FontSize(9).FontColor(Colors.Grey.Darken1).LineHeight(1.4f);
                    });

                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Generated: ").FontSize(8).FontColor(Colors.Grey.Medium);
                            text.Span($"{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private async Task CreateContactNoteAsync(Guid contactId, int noteType, string subject, string body, Guid userId, CancellationToken cancellationToken)
        {
            var note = new ContactNote
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                NoteType = noteType,
                Subject = subject,
                Body = body,
            };

            await contactNoteManager.AddAsync(note, userId, cancellationToken);
        }

        private static string GetContactDisplayName(Contact contact)
        {
            var parts = new[] { contact.FirstName, contact.LastName }.Where(s => !string.IsNullOrWhiteSpace(s));
            return string.Join(" ", parts);
        }
    }
}
