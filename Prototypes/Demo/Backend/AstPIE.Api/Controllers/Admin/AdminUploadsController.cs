using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AstPIE.Api.Data;

namespace AstPIE.Api.Controllers.Admin
{
	// Files added in the admin pages (cover images, photos, audio, video).
	// They are saved in wwwroot/uploads and served at /uploads/... Later this can move to the CDN.
	[Route("api/admin/uploads")]
	public class AdminUploadsController : AdminControllerBase
	{
		private const long MaxBytes = 100 * 1024 * 1024;

		// Only pictures, sound and video. No .svg or .html: those can carry scripts.
		private static readonly string[] AllowedExtensions = new string[]
		{
			".jpg", ".jpeg", ".png", ".webp", ".gif", ".avif",
			".mp3", ".wav", ".ogg", ".m4a",
			".mp4", ".webm"
		};

		private readonly IWebHostEnvironment environment;

		public AdminUploadsController(AppDbContext db, IWebHostEnvironment environment) : base(db)
		{
			this.environment = environment;
		}

		// POST /api/admin/uploads (form field "file")
		// Returns the address of the saved file: { "url": "http://localhost:5080/uploads/2026/sputnik-1-3f9a2c1b.jpg" }
		[HttpPost]
		[RequestSizeLimit(MaxBytes)]
		public async Task<ActionResult> Upload(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return FieldError("file", "Choose a file.");
			}
			if (file.Length > MaxBytes)
			{
				return FieldError("file", "The file is larger than 100 MB.");
			}

			string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!AllowedExtensions.Contains(extension))
			{
				return FieldError("file", "This file type is not allowed. Allowed: " + string.Join(", ", AllowedExtensions));
			}

			// A new, safe name: the original name in lower case with dashes, plus a random part
			// so two files with the same name never overwrite each other
			string baseName = Path.GetFileNameWithoutExtension(file.FileName).ToLowerInvariant();
			baseName = Regex.Replace(baseName, "[^a-z0-9]+", "-").Trim('-');
			if (baseName == "")
			{
				baseName = "file";
			}
			if (baseName.Length > 50)
			{
				baseName = baseName.Substring(0, 50);
			}
			string fileName = baseName + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;

			string year = DateTime.UtcNow.Year.ToString();
			string folder = Path.Combine(environment.WebRootPath, "uploads", year);
			Directory.CreateDirectory(folder);

			string fullPath = Path.Combine(folder, fileName);
			using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
			{
				await file.CopyToAsync(stream);
			}

			string url = Request.Scheme + "://" + Request.Host + "/uploads/" + year + "/" + fileName;
			return Created(url, new { url = url });
		}
	}
}
