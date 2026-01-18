namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.NotificationHubs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class NotificationManager : INotificationManager
    {
        private readonly ILogger<NotificationManager> logger;

        private readonly NotificationHubClient hub;

        public NotificationManager(IConfiguration configuration, ILogger<NotificationManager> logger)
        {
            var sharedAccessSignature = configuration["NotificationSharedAccessSignature"];
            var hubName = configuration["NotificationHubName"];
            hub = NotificationHubClient.CreateClientFromConnectionString(sharedAccessSignature, hubName);
            this.logger = logger;
        }

        public async Task SendMessageRequestAsync(MessageRequest messageRequest,
            CancellationToken cancellationToken = default)
        {
            // Apple requires the apns-push-type header for all requests
            var headers = new Dictionary<string, string> { { "apns-push-type", "alert" } };

            // Send the notification as a template notification. All template registrations that contain
            // "messageParam" and the proper tags will receive the notifications.
            // This includes APNS, GCM/FCM, WNS, and MPNS template registrations.

            var templateParams = new Dictionary<string, string>();

            templateParams["messageParam"] = messageRequest.Message;

            try
            {
                var result = await hub.SendTemplateNotificationAsync(templateParams, cancellationToken);

                logger.LogInformation("Result from sending notification:  {0}", JsonSerializer.Serialize(result));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to send notification");
                throw;
            }
        }
    }
}