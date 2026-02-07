using System;
using System.Collections.Generic;
using System.Text;

namespace VetLink.Services.Services.AccountService.OTPService.model
{
    public class OTPModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
