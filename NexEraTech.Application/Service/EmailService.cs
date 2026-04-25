using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexEraTech.Application.Interface;
using NexEraTech.Domain.Models;
using NexEraTech.Domain.Models.Settings;
using System.Net;
using System.Net.Mail;

namespace NexEraTech.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendAppointmentConfirmationAsync(Users user, string appointmentDetails)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    
                    var fullName = $"{user.FirstName} {user.LastName}";

                    var adminMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                        Subject = $"New Appointment Booking Request - {fullName}",
                        Body = GetAdminEmailBody(fullName, user.Email, appointmentDetails),
                        IsBodyHtml = true
                    };
                    adminMessage.To.Add(_emailSettings.SenderEmail);

                    var userMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                        Subject = "NexEra Tech: Consultation Appointment Confirmation",
                        Body = GetUserEmailBody(fullName, appointmentDetails, user.AppointmentDate),
                        IsBodyHtml = true
                    };
                    userMessage.To.Add(user.Email);

                    await client.SendMailAsync(adminMessage);
                    _logger.LogInformation("Admin notification email sent successfully");

                    _logger.LogInformation("Sending user confirmation email to: {RecipientEmail}", user.Email);
                    await client.SendMailAsync(userMessage);
                    _logger.LogInformation("User confirmation email sent successfully to: {RecipientEmail}", user.Email);

                    adminMessage.Dispose();
                    userMessage.Dispose();

                    _logger.LogInformation("Appointment confirmation emails completed successfully for: {RecipientEmail}", user.Email);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending appointment confirmation emails for {RecipientEmail}. Error: {ErrorMessage}",
                    user.Email, ex.Message);
                return false;
            }
        }

        private string GetAdminEmailBody(string recipientName, string recipientEmail, string appointmentDetails)
        {
            recipientName = string.IsNullOrWhiteSpace(recipientName) ? "Missing Recipient Name" : recipientName;
            recipientEmail = string.IsNullOrWhiteSpace(recipientEmail) ? "No Email Provided" : recipientEmail;
            appointmentDetails = string.IsNullOrWhiteSpace(appointmentDetails)
                ? "No appointment details were provided."
                : appointmentDetails;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #334155; margin: 0; padding: 20px; background-color: #f1f5f9; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e2e8f0; border-radius: 4px; overflow: hidden; }}
        .header {{ background-color: #0f172a; color: #ffffff; padding: 24px; text-align: left; }}
        .header h1 {{ margin: 0; font-size: 20px; font-weight: 600; letter-spacing: 0.5px; }}
        .content {{ padding: 32px 24px; }}
        .details-table {{ width: 100%; border-collapse: collapse; margin-top: 16px; margin-bottom: 24px; }}
        .details-table th, .details-table td {{ padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: left; font-size: 14px; }}
        .details-table th {{ width: 30%; color: #64748b; font-weight: 600; }}
        .details-table td {{ color: #0f172a; font-weight: 500; }}
        .notes-section {{ background-color: #f8fafc; padding: 16px; border-left: 4px solid #cbd5e1; font-size: 14px; }}
        .footer {{ background-color: #f8fafc; padding: 16px 24px; text-align: center; color: #64748b; font-size: 12px; border-top: 1px solid #e2e8f0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>New Appointment Booking</h1>
        </div>
        
        <div class=""content"">
            <p style=""margin-top: 0;"">A new consultation appointment has been requested.</p>
            
            <table class=""details-table"">
                <tr>
                    <th>Client Name</th>
                    <td>{recipientName}</td>
                </tr>
                <tr>
                    <th>Email Address</th>
                    <td><a href=""mailto:{recipientEmail}"" style=""color: #0284c7; text-decoration: none;"">{recipientEmail}</a></td>
                </tr>
            </table>

            <h3 style=""font-size: 16px; color: #0f172a; margin-bottom: 8px;"">Appointment Details</h3>
            <div class=""notes-section"">
                {appointmentDetails}
            </div>

            <p style=""font-size: 14px; margin-top: 24px; color: #475569;""><strong>Action Required:</strong> Please review the details above and reach out to the client within 1 business day to confirm the schedule.</p>
        </div>

        <div class=""footer"">
            &copy; 2026 NexEra Tech. Internal Communication.
        </div>
    </div>
</body>
</html>
";
        }

        private string GetUserEmailBody(string recipientName, string appointmentDetails, DateTime appointmentDate)
        {
            recipientName = string.IsNullOrWhiteSpace(recipientName) ? "Valued Client" : recipientName;
            appointmentDetails = string.IsNullOrWhiteSpace(appointmentDetails)
                ? "Additional details regarding your session will be provided shortly."
                : appointmentDetails;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #334155; margin: 0; padding: 20px; background-color: #f1f5f9; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e2e8f0; border-radius: 4px; overflow: hidden; }}
        .header {{ background-color: #0f172a; color: #ffffff; padding: 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 22px; font-weight: 600; letter-spacing: 0.5px; }}
        .content {{ padding: 32px 24px; }}
        .session-details {{ background-color: #f8fafc; border: 1px solid #e2e8f0; padding: 20px; border-radius: 4px; margin: 24px 0; }}
        .session-details h3 {{ margin-top: 0; color: #0f172a; font-size: 16px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px; margin-bottom: 16px; text-transform: uppercase; letter-spacing: 0.5px; }}
        .next-steps h3 {{ color: #0f172a; font-size: 16px; margin-top: 24px; border-bottom: 1px solid #e2e8f0; padding-bottom: 8px; text-transform: uppercase; letter-spacing: 0.5px; }}
        .next-steps ol {{ padding-left: 20px; color: #475569; font-size: 15px; margin-top: 16px; }}
        .next-steps li {{ margin-bottom: 12px; }}
        .footer {{ background-color: #f8fafc; padding: 24px; text-align: center; color: #64748b; font-size: 12px; border-top: 1px solid #e2e8f0; }}
        .footer a {{ color: #0284c7; text-decoration: none; font-weight: 500; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Appointment Confirmation</h1>
        </div>
        
        <div class=""content"">
            <p>Dear {recipientName},</p>
            
            <p>Thank you for requesting a consultation with <strong>NexEra Tech</strong>. We have successfully received your request and look forward to discussing your business objectives.</p>

            <div class=""session-details"">
                <h3>Session Details</h3>
                <p style=""margin: 0; font-size: 15px;""><strong>Scheduled Date & Time:</strong><br>{appointmentDate.ToString("f")}</p>
            </div>

            <div class=""next-steps"">
                <h3>What to Expect Next</h3>
                <ol>
                    <li><strong>Confirmation:</strong> A representative from our team will contact you within 1 business day to formalize the appointment.</li>
                    <li><strong>Preparation:</strong> You may receive a brief questionnaire to ensure we maximize our time during the consultation.</li>
                    <li><strong>Consultation:</strong> A 30-minute strategic session focused on your specific goals and requirements.</li>
                    <li><strong>Proposal:</strong> Following our meeting, we will provide a comprehensive, tailored proposal for your review.</li>
                </ol>
            </div>

            <p style=""margin-top: 32px;"">Should you have any immediate questions, please reply directly to this email or contact us at <a href=""mailto:{_emailSettings.SenderEmail}"" style=""color: #0284c7; text-decoration: none;"">{_emailSettings.SenderEmail}</a>.</p>

            <p style=""margin-top: 32px; margin-bottom: 0;"">Sincerely,<br><strong>The NexEra Tech Team</strong></p>
        </div>

        <div class=""footer"">
            <p style=""margin: 0 0 8px 0;"">&copy; 2026 NexEra Tech. All rights reserved.</p>
            <p style=""margin: 0;""><a href=""https://nexeratech.us"">Visit our Corporate Website</a></p>
        </div>
    </div>
</body>
</html>
";
        }
    }
}
