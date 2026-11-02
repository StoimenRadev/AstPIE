using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AstPIE.Api.Security
{
	// There are no user accounts. Writing is allowed only with the one admin key,
	// sent in the X-Admin-Key header. The key is in appsettings.Local.json (never in git).
	public class AdminKeyAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public const string SchemeName = "AdminKey";
		public const string PolicyName = "Admin";
		public const string HeaderName = "X-Admin-Key";

		private readonly IConfiguration configuration;

		public AdminKeyAuthentication(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			IConfiguration configuration) : base(options, logger, encoder)
		{
			this.configuration = configuration;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			string expectedKey = configuration["Admin:Key"];

			// No key set on the server: nothing can be written
			if (string.IsNullOrEmpty(expectedKey))
			{
				return Task.FromResult(AuthenticateResult.Fail("No admin key is set on the server."));
			}

			string sentKey = Request.Headers[HeaderName].ToString();
			if (sentKey == "")
			{
				return Task.FromResult(AuthenticateResult.NoResult());
			}

			// Compared in constant time, so the key can't be guessed from how long the check takes
			byte[] expected = Encoding.UTF8.GetBytes(expectedKey);
			byte[] sent = Encoding.UTF8.GetBytes(sentKey);
			if (!CryptographicOperations.FixedTimeEquals(expected, sent))
			{
				return Task.FromResult(AuthenticateResult.Fail("Wrong admin key."));
			}

			Claim[] claims = new Claim[] { new Claim(ClaimTypes.Role, "Admin") };
			ClaimsIdentity identity = new ClaimsIdentity(claims, SchemeName);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, SchemeName);
			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}
