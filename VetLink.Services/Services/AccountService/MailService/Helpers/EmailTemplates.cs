using VetLink.Services.Services.AccountService.MailService.Dtos;

namespace VetLink.Services.Services.Email
{
    public static class EmailTemplates
    {
        private static string _baseUrl = "https://vetlink.com";

        public static void Configure(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        #region OTP Email Templates
        public static (string subject, string body) GetOtpVerificationEmail(OtpEmailModel model)
        {
            var subject = "🔐 Your VetLink Verification Code";
            var body = BuildOtpEmailHtml(model);
            return (subject, body);
        }

        public static (string subject, string body) GetOtpVerificationSuccessEmail(EmailModel model)
        {
            var subject = "✅ VetLink - OTP Verified Successfully";
            var body = BuildOtpSuccessEmailHtml(model);
            return (subject, body);
        }
        public static (string subject, string body) GetOtpVerificationSuccessSellerEmail(EmailModel model)
        {
            var subject = "✅ VetLink - OTP Verified Successfully";
            var body = BuildOtpSuccessSellerEmailHtml(model);
            return (subject, body);
        }
        #endregion

        #region Welcome Email Templates
        public static (string subject, string body) GetWelcomeEmail(WelcomeEmailModel model)
        {
            var subject = "🎉 Welcome to VetLink - Your Account is Ready!";
            var body = BuildWelcomeEmailHtml(model);
            return (subject, body);
        }
        #endregion

        #region Seller Approval/Rejection Templates
        public static (string subject, string body) GetSellerApprovalEmail(EmailModel model)
        {
            var subject = "✅ VetLink - Seller Account Approved";
            var body = BuildSellerApprovalEmailHtml(model);
            return (subject, body);
        }

        public static (string subject, string body) GetSellerRejectionEmail(SellerRejectionEmailModel model)
        {
            var subject = "❌ VetLink - Seller Account Rejected";
            var body = BuildSellerRejectionEmailHtml(model);
            return (subject, body);
        }

        private static string BuildSellerApprovalEmailHtml(EmailModel model)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; background: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 5px 15px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 40px 20px; text-align: center; }}
        .content {{ padding: 40px; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 14px 35px; text-decoration: none; border-radius: 8px; font-weight: bold; margin: 25px 0; transition: transform 0.2s; }}
        .button:hover {{ transform: translateY(-2px); box-shadow: 0 5px 15px rgba(76, 175, 80, 0.4); }}
        .footer {{ margin-top: 40px; padding-top: 25px; border-top: 1px solid #eaeaea; color: #666; font-size: 14px; }}
        .dashboard-section {{ background: #f9f9f9; border-radius: 8px; padding: 25px; margin: 25px 0; border-left: 4px solid #4CAF50; }}
        .feature-list {{ margin: 20px 0; padding-left: 20px; }}
        .feature-item {{ margin-bottom: 12px; color: #555; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0; font-size: 28px;'>🎉 Seller Account Approved!</h1>
            <p style='margin: 10px 0 0 0; font-size: 18px; opacity: 0.9;'>Welcome to VetLink Seller Community</p>
        </div>
        <div class='content'>
            <p>Dear {EscapeHtml(model.Name) ?? "Seller"},</p>
            <p>Great news! Your seller account has been <strong>approved</strong> by the VetLink administration team.</p>
            <p>You can now access your seller dashboard and start listing your products.</p>
            
            <div class='dashboard-section'>
                <h3 style='margin-top: 0; color: #2E7D32;'>What's Next?</h3>
                <ul class='feature-list'>
                    <li class='feature-item'><strong>📦 List Products</strong> - Start adding your products to the marketplace</li>
                    <li class='feature-item'><strong>🏪 Store Setup</strong> - Customize your store profile and branding</li>
                    <li class='feature-item'><strong>📊 Analytics</strong> - Track your sales and customer insights</li>
                    <li class='feature-item'><strong>💬 Support</strong> - Access 24/7 seller support</li>
                </ul>
            </div>
            
            <div style='text-align: center;'>
                <a href='{_baseUrl}/seller/dashboard' class='button'>Go to Seller Dashboard</a>
            </div>
            
            <p><strong>Important Information:</strong></p>
            <ul>
                <li>Review our <a href='{_baseUrl}/seller/guidelines'>Seller Guidelines</a></li>
                <li>Set up your payment information to receive payments</li>
                <li>Download our Seller App for mobile management</li>
            </ul>
            
            <p>If you have any questions, our seller support team is here to help at <a href='mailto:seller-support@vetlink.com'>seller-support@vetlink.com</a>.</p>
            
            <div class='footer'>
                <p>Best regards,<br><strong>The VetLink Seller Team</strong></p>
                <p style='font-size: 12px; color: #888; margin-top: 20px;'>
                    This is an automated message. Please do not reply to this email.<br>
                    © {DateTime.Now.Year} VetLink Seller Network. All rights reserved.
                </p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private static string BuildSellerRejectionEmailHtml(SellerRejectionEmailModel model)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; background: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 5px 15px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #f44336, #d32f2f); color: white; padding: 40px 20px; text-align: center; }}
        .content {{ padding: 40px; }}
        .rejection-box {{ background: #ffebee; border-left: 4px solid #f44336; padding: 20px; margin: 25px 0; border-radius: 4px; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 14px 35px; text-decoration: none; border-radius: 8px; font-weight: bold; margin: 25px 0; transition: transform 0.2s; }}
        .button:hover {{ transform: translateY(-2px); box-shadow: 0 5px 15px rgba(33, 150, 243, 0.4); }}
        .footer {{ margin-top: 40px; padding-top: 25px; border-top: 1px solid #eaeaea; color: #666; font-size: 14px; }}
        .improvement-tips {{ background: #f9f9f9; border-radius: 8px; padding: 25px; margin: 25px 0; }}
        .tip-item {{ margin-bottom: 12px; padding-left: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0; font-size: 28px;'>Seller Account Review Status</h1>
            <p style='margin: 10px 0 0 0; font-size: 18px; opacity: 0.9;'>Important Update Regarding Your Application</p>
        </div>
        <div class='content'>
            <p>Dear {EscapeHtml(model.Name) ?? "Applicant"},</p>
            <p>Thank you for your interest in becoming a seller on VetLink. After careful review of your application, we regret to inform you that we are unable to approve your seller account at this time.</p>
            
            <div class='rejection-box'>
                <h3 style='margin-top: 0; color: #d32f2f;'>Reason for Rejection:</h3>
                <p style='font-style: italic;'>{EscapeHtml(model.Reason)}</p>
            </div>
            
            <p>We understand this news may be disappointing, but we encourage you to address the issues mentioned above and reapply in the future.</p>
            
            <div class='improvement-tips'>
                <h3 style='margin-top: 0; color: #1976D2;'>How to Improve Your Application:</h3>
                <div class='tip-item'><strong>📋 Complete Documentation</strong> - Ensure all required documents are provided and clearly legible</div>
                <div class='tip-item'><strong>🏪 Business Information</strong> - Provide detailed and accurate business information</div>
                <div class='tip-item'><strong>📊 Product Quality</strong> - Focus on quality products that meet our marketplace standards</div>
                <div class='tip-item'><strong>📝 Clear Policies</strong> - Establish clear return, shipping, and customer service policies</div>
            </div>
            
            <div style='text-align: center;'>
                <a href='{_baseUrl}/seller/apply' class='button'>Reapply in 30 Days</a>
            </div>
            
            <p><strong>Need Assistance?</strong></p>
            <p>If you need clarification or have questions about our decision, please contact our seller relations team:</p>
            <ul>
                <li>Email: <a href='mailto:seller-relations@vetlink.com'>seller-relations@vetlink.com</a></li>
                <li>Phone: +1 (555) 123-4567 (Monday-Friday, 9 AM - 6 PM EST)</li>
                <li>Review our <a href='{_baseUrl}/seller/requirements'>Seller Requirements</a> page</li>
            </ul>
            
            <p>We appreciate your interest in VetLink and hope to review your application again in the future.</p>
            
            <div class='footer'>
                <p>Best regards,<br><strong>The VetLink Seller Review Team</strong></p>
                <p style='font-size: 12px; color: #888; margin-top: 20px;'>
                    This decision is final for the current application.<br>
                    © {DateTime.Now.Year} VetLink Marketplace. All rights reserved.
                </p>
            </div>
        </div>
    </div>
</body>
</html>";
        }
        #endregion

        #region Password Reset Email Templates
        public static (string subject, string body) GetPasswordResetEmail(PasswordResetEmailModel model)
        {
            var subject = "🔒 VetLink - Password Reset Request";
            var body = BuildPasswordResetEmailHtml(model);
            return (subject, body);
        }

        public static (string subject, string body) GetPasswordResetSuccessEmail(EmailModel model)
        {
            var subject = "✅ VetLink - Password Reset Successful";
            var body = BuildPasswordResetSuccessEmailHtml(model);
            return (subject, body);
        }
        #endregion

        #region Appointment Email Templates
        public static (string subject, string body) GetAppointmentConfirmationEmail(AppointmentEmailModel model)
        {
            var subject = "📅 VetLink - Appointment Confirmed";
            var body = BuildAppointmentConfirmationEmailHtml(model);
            return (subject, body);
        }
        #endregion

        #region Generic Templates
        public static (string subject, string body) GetSimpleNotificationEmail(
            string recipientName,
            string title,
            string message,
            string? actionText = null,
            string? actionUrl = null)
        {
            var subject = $"📬 VetLink - {title}";
            var body = BuildSimpleNotificationEmailHtml(recipientName, title, message, actionText, actionUrl);
            return (subject, body);
        }
        #endregion

        #region Private HTML Builders
        private static string BuildOtpEmailHtml(OtpEmailModel model)
        {
            // Add the nonce information to the email
            var nonceHtml = string.IsNullOrEmpty(model.Nonce) ? "" :
                $@"
        <p><strong>Verification Reference:</strong> {model.Nonce}</p>
        <p><em>You may need this reference when entering the code.</em></p>";

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 30px; text-align: center; }}
        .content {{ padding: 30px; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #667eea; text-align: center; letter-spacing: 5px; margin: 20px 0; padding: 15px; background: #f0f4ff; border-radius: 8px; }}
        .reference {{ font-size: 14px; color: #666; background: #f9f9f9; padding: 10px; border-radius: 5px; border-left: 3px solid #667eea; margin: 20px 0; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>VetLink Security Code</h1>
            <p>Use this code to verify your email</p>
        </div>
        <div class='content'>
            <p>Hello {(string.IsNullOrEmpty(model.Name) || model.Name == "string" ? "there" : model.Name)},</p>
            <p>To complete your email verification, please use the following One-Time Password (OTP):</p>
            <div class='otp-code'><strong>{model.OtpCode}</strong></div>
            <div class='reference'>
                <p><strong>Security Note:</strong> This OTP is tied to your session.</p>
                {nonceHtml}
            </div>
            <p><strong>This code will expire in {model.OtpExpiryMinutes} minutes.</strong></p>
            <p>If you didn't request this code, please ignore this email or contact our support team immediately.</p>
            <p>For security reasons, never share this code with anyone.</p>
            <div class='footer'>
                <p>Best regards,<br>The VetLink Security Team</p>
                <p style='font-size: 12px; color: #888;'>
                    This is an automated message. Please do not reply.<br>
                    © {DateTime.Now.Year} VetLink. All rights reserved.
                </p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private static string BuildOtpSuccessEmailHtml(EmailModel model)
        {
            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #4CAF50; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='margin: 0;'>OTP Verified Successfully!</h1>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(model.Name) ?? "there"},</p>
        <p>Your OTP has been verified successfully. Your account is now secure and ready to use.</p>
        <p>You can proceed to login with your credentials.</p>
        <p>Best regards,<br>VetLink Team</p>
    </div>
</div>";
        }
        private static string BuildOtpSuccessSellerEmailHtml(EmailModel model)
        {
            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #4CAF50; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='margin: 0;'>OTP Verified Successfully!</h1>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(model.Name) ?? "there"},</p>
        <p>Your OTP has been verified successfully. Your account is now secure and ready to use.</p>
        <p>Wait for Admin Approval</p>
        <p>Best regards,<br>VetLink Team</p>
    </div>
</div>";
        }

        private static string BuildWelcomeEmailHtml(WelcomeEmailModel model)
        {
            var dashboardUrl = model.DashboardUrl ?? $"{_baseUrl}/dashboard";
            var supportEmail = model.SupportEmail ?? "support@vetlink.com";

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; background: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 5px 15px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 40px 20px; text-align: center; }}
        .content {{ padding: 40px; }}
        .welcome-text {{ font-size: 18px; margin-bottom: 25px; color: #444; }}
        .button {{ display: inline-block; background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 14px 35px; text-decoration: none; border-radius: 8px; font-weight: bold; margin: 25px 0; transition: transform 0.2s; }}
        .button:hover {{ transform: translateY(-2px); box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4); }}
        .features {{ margin: 30px 0; }}
        .feature-item {{ margin-bottom: 15px; padding-left: 10px; border-left: 3px solid #667eea; }}
        .footer {{ margin-top: 40px; padding-top: 25px; border-top: 1px solid #eaeaea; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0; font-size: 28px;'>Welcome to VetLink! 🐾</h1>
            <p style='margin: 10px 0 0 0; font-size: 18px; opacity: 0.9;'>Your veterinary practice management platform</p>
        </div>
        <div class='content'>
            <p class='welcome-text'>Hello {EscapeHtml(model.Name) ?? "there"},</p>
            <p>We're thrilled to welcome you to VetLink! Your account has been successfully verified and is now ready to use.</p>
            <p>VetLink is designed to help veterinary professionals like you streamline practice management, enhance patient care, and grow your business.</p>
            <div class='features'>
                <div class='feature-item'><strong>📋 Practice Management</strong> - Schedule appointments, manage patient records, and track treatments</div>
                <div class='feature-item'><strong>💊 Inventory Control</strong> - Manage medications and supplies with smart alerts</div>
                <div class='feature-item'><strong>💰 Billing & Invoicing</strong> - Process payments and generate invoices seamlessly</div>
                <div class='feature-item'><strong>📊 Analytics Dashboard</strong> - Get insights into your practice performance</div>
                <div class='feature-item'><strong>📱 Mobile Access</strong> - Manage your practice on-the-go with our mobile app</div>
            </div>
            <div style='text-align: center;'>
                <a href='{dashboardUrl}' class='button'>Launch Your Dashboard</a>
            </div>
            <p><strong>Getting Started:</strong></p>
            <ol style='color: #555;'>
                <li>Complete your practice profile</li>
                <li>Add your team members</li>
                <li>Set up your services and pricing</li>
                <li>Import or add your patient records</li>
                <li>Explore our training resources</li>
            </ol>
            <p>Need help? Our support team is available at <a href='mailto:{supportEmail}'>{supportEmail}</a> or check out our <a href='{_baseUrl}/help-center'>Help Center</a>.</p>
            <div class='footer'>
                <p>Best regards,<br><strong>The VetLink Team</strong></p>
                <p style='font-size: 12px; color: #888; margin-top: 20px;'>
                    This is an automated message. Please do not reply to this email.<br>
                    © {DateTime.Now.Year} VetLink. All rights reserved.<br>
                    <a href='{_baseUrl}/privacy' style='color: #667eea;'>Privacy Policy</a> | 
                    <a href='{_baseUrl}/terms' style='color: #667eea;'>Terms of Service</a>
                </p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private static string BuildPasswordResetEmailHtml(PasswordResetEmailModel model)
        {
            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #ff6b6b; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='margin: 0;'>Password Reset Request</h1>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(model.Name) ?? "there"},</p>
        <p>We received a request to reset your VetLink account password.</p>
        <div style='text-align: center; margin: 25px 0;'>
            <a href='{model.ResetLink}' style='background: #ff6b6b; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;'>
                Reset Password
            </a>
        </div>
        <p><strong>This link will expire in {model.LinkExpiryHours} hours.</strong></p>
        <p>If you didn't request a password reset, please ignore this email or contact our support team immediately.</p>
        <p>Best regards,<br>VetLink Security Team</p>
    </div>
</div>";
        }

        private static string BuildPasswordResetSuccessEmailHtml(EmailModel model)
        {
            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #4CAF50; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='margin: 0;'>Password Reset Successful</h1>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(model.Name) ?? "there"},</p>
        <p>Your VetLink password has been successfully reset.</p>
        <p>If you did not make this change, please contact our support team immediately.</p>
        <p>For security, we recommend using a strong, unique password and enabling two-factor authentication.</p>
        <p>Best regards,<br>VetLink Security Team</p>
    </div>
</div>";
        }

        private static string BuildAppointmentConfirmationEmailHtml(AppointmentEmailModel model)
        {
            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #36d1dc; background: linear-gradient(135deg, #36d1dc, #5b86e5); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='margin: 0;'>Appointment Confirmed</h1>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(model.Name) ?? "there"},</p>
        <p>Your appointment has been successfully scheduled.</p>
        <div style='background: white; border-left: 4px solid #36d1dc; padding: 20px; margin: 20px 0; border-radius: 4px;'>
            <p><strong>Appointment ID:</strong> {EscapeHtml(model.AppointmentId)}</p>
            <p><strong>Date & Time:</strong> {model.AppointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
            <p><strong>Pet:</strong> {EscapeHtml(model.PetName)}</p>
            <p><strong>Veterinarian:</strong> {EscapeHtml(model.VeterinarianName)}</p>
        </div>
        <p>Please arrive 10 minutes before your scheduled time.</p>
        <p>To reschedule or cancel, please contact us at least 24 hours in advance.</p>
        <p>Best regards,<br>VetLink Team</p>
    </div>
</div>";
        }

        private static string BuildSimpleNotificationEmailHtml(
            string recipientName,
            string title,
            string message,
            string? actionText = null,
            string? actionUrl = null)
        {
            var actionHtml = string.Empty;
            if (!string.IsNullOrEmpty(actionText) && !string.IsNullOrEmpty(actionUrl))
            {
                actionHtml = $@"
                <div style='text-align: center; margin: 20px 0;'>
                    <a href='{actionUrl}' style='background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                        {EscapeHtml(actionText)}
                    </a>
                </div>";
            }

            return $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <div style='background: #667eea; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h2 style='margin: 0;'>{EscapeHtml(title)}</h2>
    </div>
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <p>Hello {EscapeHtml(recipientName)},</p>
        <p>{EscapeHtml(message)}</p>
        {actionHtml}
        <p>Best regards,<br>VetLink Team</p>
    </div>
</div>";
        }

        private static string? EscapeHtml(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Net.WebUtility.HtmlEncode(input);
        }
        #endregion
    }
}