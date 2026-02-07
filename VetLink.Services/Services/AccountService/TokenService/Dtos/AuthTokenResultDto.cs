using System;
using System.Collections.Generic;
using System.Text;

namespace VetLink.Services.Services.AccountService.TokenService.Dtos
{
	public class AuthTokenResultDto
	{
		public string AccessToken { get; set; }
		public DateTime? ExpiresAt { get; set; }
		public string RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiresAt { get; set; }
	}
}
