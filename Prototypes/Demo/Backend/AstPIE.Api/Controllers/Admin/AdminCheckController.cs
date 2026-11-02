using Microsoft.AspNetCore.Mvc;
using AstPIE.Api.Data;

namespace AstPIE.Api.Controllers.Admin
{
	[Route("api/admin/check")]
	public class AdminCheckController : AdminControllerBase
	{
		public AdminCheckController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/check
		// 204 when the admin key is right, 401 when not. Used by the admin login.
		[HttpGet]
		public IActionResult Check()
		{
			return NoContent();
		}
	}
}
