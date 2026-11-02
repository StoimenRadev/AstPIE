using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AstPIE.Api.Data;
using AstPIE.Api.Dtos;
using AstPIE.Api.Models;

namespace AstPIE.Api.Controllers.Admin
{
	[Route("api/admin/media")]
	public class AdminMediaController : AdminControllerBase
	{
		public AdminMediaController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/media or /api/admin/media?entryId=3
		[HttpGet]
		public async Task<ActionResult<List<MediaItemAdminDto>>> GetAll(int? entryId)
		{
			return await (
				from m in db.MediaItems
				where entryId == null || m.EntryId == entryId
				orderby m.EntryId, m.SortOrder
				select new MediaItemAdminDto
				{
					Id = m.Id,
					EntryId = m.EntryId,
					Type = m.Type,
					Url = m.Url,
					Title = m.Title,
					Caption = m.Caption,
					Credit = m.Credit,
					License = m.License,
					SortOrder = m.SortOrder
				}).ToListAsync();
		}

		// GET /api/admin/media/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<MediaItemAdminDto>> GetOne(int id)
		{
			MediaItemAdminDto media = await (
				from m in db.MediaItems
				where m.Id == id
				select new MediaItemAdminDto
				{
					Id = m.Id,
					EntryId = m.EntryId,
					Type = m.Type,
					Url = m.Url,
					Title = m.Title,
					Caption = m.Caption,
					Credit = m.Credit,
					License = m.License,
					SortOrder = m.SortOrder
				}).FirstOrDefaultAsync();

			if (media == null)
			{
				return NotFound();
			}
			return media;
		}

		// POST /api/admin/media
		[HttpPost]
		public async Task<ActionResult> Create(MediaItemInput input)
		{
			if (!await EntryExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}

			MediaItem media = new MediaItem();
			Apply(input, media);
			db.MediaItems.Add(media);
			await db.SaveChangesAsync();
			return Created("/api/admin/media/" + media.Id, new { id = media.Id });
		}

		// PUT /api/admin/media/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, MediaItemInput input)
		{
			MediaItem media = await db.MediaItems.FindAsync(id);
			if (media == null)
			{
				return NotFound();
			}
			if (!await EntryExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}

			Apply(input, media);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/media/5
		// Removes the database row. An uploaded file stays in wwwroot/uploads.
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			MediaItem media = await db.MediaItems.FindAsync(id);
			if (media == null)
			{
				return NotFound();
			}

			db.MediaItems.Remove(media);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> EntryExists(int entryId)
		{
			return await (from e in db.Entries where e.Id == entryId select e).AnyAsync();
		}

		private static void Apply(MediaItemInput input, MediaItem media)
		{
			media.EntryId = input.EntryId;
			media.Type = input.Type;
			media.Url = input.Url.Trim();
			media.Title = input.Title.Trim();
			media.Caption = Optional(input.Caption);
			media.Credit = Optional(input.Credit);
			media.License = Optional(input.License);
			media.SortOrder = input.SortOrder;
		}
	}
}
