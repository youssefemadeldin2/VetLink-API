using System;
using System.Collections.Generic;
using System.Text;

namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class AuthResponseDto
    {
		public string AccessToken { get; set; }
		public DateTime? ExpiresAt { get; set; }

		public string RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiresAt { get; set; }

		public UserDto User { get; set; }
	}
}
