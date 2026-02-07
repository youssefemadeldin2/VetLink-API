using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class OtpVerificationDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Nonce { get; set; } = null!;
        [Required]
        public string OTP { get; set; } = null!;
    }
}
