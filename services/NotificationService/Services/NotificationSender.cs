using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationSender
    {
        Task SendAsync(Notification notification);
    }

    public class NotificationSender : INotificationSender
    {
        private readonly ILogger<NotificationSender> _logger;
        private readonly IConfiguration _configuration;

        public NotificationSender(ILogger<NotificationSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendAsync(Notification notification)
        {
            try
            {
                switch (notification.Channel.ToLower())
                {
                    case "email":
                        await SendEmail(notification);
                        break;
                    case "sms":
                        await SendSms(notification);
                        break;
                    case "push":
                        await SendPushNotification(notification);
                        break;
                    case "inapp":
                        // In-app notifications are just stored in DB
                        _logger.LogInformation($"In-app notification created: {notification.Id}");
                        break;
                    default:
                        _logger.LogWarning($"Unknown notification channel: {notification.Channel}");
                        break;
                }

                notification.Status = "Delivered";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification {notification.Id}");
                notification.Status = "Failed";
                notification.FailureReason = ex.Message;
                notification.RetryCount++;
            }
        }

        private async Task SendEmail(Notification notification)
        {
            // Implement email sending logic using SMTP or email service (SendGrid, AWS SES, etc.)
            _logger.LogInformation($"Sending email to {notification.RecipientEmail}");
            _logger.LogInformation($"Subject: {notification.Subject}");
            _logger.LogInformation($"Message: {notification.Message}");

            // Simulate email sending
            await Task.Delay(100);

            // In production, use actual email service:
            // var client = new SmtpClient(...);
            // await client.SendMailAsync(...);
        }

        private async Task SendSms(Notification notification)
        {
            // Implement SMS sending logic using Twilio, AWS SNS, etc.
            _logger.LogInformation($"Sending SMS to {notification.RecipientPhone}");
            _logger.LogInformation($"Message: {notification.Message}");

            // Simulate SMS sending
            await Task.Delay(100);

            // In production, use actual SMS service:
            // var twilioClient = new TwilioRestClient(...);
            // await twilioClient.SendMessageAsync(...);
        }

        private async Task SendPushNotification(Notification notification)
        {
            // Implement push notification logic using Firebase, OneSignal, etc.
            _logger.LogInformation($"Sending push notification to user {notification.RecipientId}");
            _logger.LogInformation($"Title: {notification.Subject}");
            _logger.LogInformation($"Body: {notification.Message}");

            // Simulate push notification
            await Task.Delay(100);

            // In production, use actual push service:
            // var firebaseClient = new FirebaseClient(...);
            // await firebaseClient.SendAsync(...);
        }
    }
}