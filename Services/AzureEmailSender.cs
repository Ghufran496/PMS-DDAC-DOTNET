using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace PMS_DDAC.Services
{
    public class AzureEmailSender : IEmailSender
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;

        public AzureEmailSender(IConfiguration configuration)
        {
            var connectionString = configuration["AzureEmail:ConnectionString"];
            _senderAddress = configuration["AzureEmail:SenderEmail"] ?? throw new ArgumentNullException(nameof(_senderAddress), "SenderEmail cannot be null.");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Azure Email ConnectionString cannot be null or empty.");
            }

            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new EmailMessage(
                senderAddress: _senderAddress,
                content: new EmailContent(subject)
                {
                    PlainText = "This is an Azure Email message.",
                    Html = htmlMessage
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(email) })
            );

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage
            );

            Console.WriteLine($"Email Sent. Status: {emailSendOperation.Value.Status}");
        }
    }
}
