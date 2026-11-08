using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AstPIE.Api.Data;
using AstPIE.Api.Security;

namespace AstPIE.Api.Controllers.Admin
{
	// Everything under /api/admin needs the admin key and is rate limited.
	// Without the key: 401. Wrong data: 400. Duplicate slug or blocked delete: 409.
	[ApiController]
	[Authorize(Policy = AdminKeyAuthentication.PolicyName)]
	[EnableRateLimiting("admin")]
	public abstract class AdminControllerBase : ControllerBase
	{
		protected readonly AppDbContext db;

		protected AdminControllerBase(AppDbContext db)
		{
			this.db = db;
		}

		// 409 with a message the admin page can show
		protected ObjectResult ConflictMessage(string message)
		{
			return Problem(message, null, StatusCodes.Status409Conflict, "Conflict");
		}

		// 400 for one field, in the same format as the automatic checks
		protected ActionResult FieldError(string field, string message)
		{
			ModelState.AddModelError(field, message);
			return ValidationProblem(ModelState);
		}

		// Optional text for a column that may be empty (null)
		protected static string Optional(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return text.Trim();
		}

		// A list of lines (one item per line in the form): empty lines left out, each line trimmed
		protected static List<string> CleanList(List<string> items)
		{
			List<string> result = new List<string>();
			if (items == null)
			{
				return result;
			}
			foreach (string item in items)
			{
				if (!string.IsNullOrWhiteSpace(item))
				{
					result.Add(item.Trim());
				}
			}
			return result;
		}

		// Optional text for a column that can't be null: "" when nothing was given
		protected static string TextOrEmpty(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return "";
			}
			return text.Trim();
		}
	}
}
