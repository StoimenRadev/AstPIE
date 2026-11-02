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
	[Route("api/admin/chapters")]
	public class AdminChaptersController : AdminControllerBase
	{
		public AdminChaptersController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/chapters or /api/admin/chapters?entryId=3
		[HttpGet]
		public async Task<ActionResult<List<ChapterAdminDto>>> GetAll(int? entryId)
		{
			return await (
				from c in db.Chapters
				where entryId == null || c.EntryId == entryId
				orderby c.EntryId, c.SortOrder
				select new ChapterAdminDto
				{
					Id = c.Id,
					EntryId = c.EntryId,
					Title = c.Title,
					DateLabel = c.DateLabel,
					Summary = c.Summary,
					Body = c.Body,
					SortOrder = c.SortOrder
				}).ToListAsync();
		}

		// GET /api/admin/chapters/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<ChapterAdminDto>> GetOne(int id)
		{
			ChapterAdminDto chapter = await (
				from c in db.Chapters
				where c.Id == id
				select new ChapterAdminDto
				{
					Id = c.Id,
					EntryId = c.EntryId,
					Title = c.Title,
					DateLabel = c.DateLabel,
					Summary = c.Summary,
					Body = c.Body,
					SortOrder = c.SortOrder
				}).FirstOrDefaultAsync();

			if (chapter == null)
			{
				return NotFound();
			}
			return chapter;
		}

		// POST /api/admin/chapters
		[HttpPost]
		public async Task<ActionResult> Create(ChapterInput input)
		{
			if (!await EntryExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}

			Chapter chapter = new Chapter();
			Apply(input, chapter);
			db.Chapters.Add(chapter);
			await db.SaveChangesAsync();
			return Created("/api/admin/chapters/" + chapter.Id, new { id = chapter.Id });
		}

		// PUT /api/admin/chapters/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, ChapterInput input)
		{
			Chapter chapter = await db.Chapters.FindAsync(id);
			if (chapter == null)
			{
				return NotFound();
			}
			if (!await EntryExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}

			Apply(input, chapter);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/chapters/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Chapter chapter = await db.Chapters.FindAsync(id);
			if (chapter == null)
			{
				return NotFound();
			}

			db.Chapters.Remove(chapter);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> EntryExists(int entryId)
		{
			return await (from e in db.Entries where e.Id == entryId select e).AnyAsync();
		}

		private static void Apply(ChapterInput input, Chapter chapter)
		{
			chapter.EntryId = input.EntryId;
			chapter.Title = input.Title.Trim();
			chapter.DateLabel = Optional(input.DateLabel);
			chapter.Summary = input.Summary.Trim();
			chapter.Body = input.Body;
			chapter.SortOrder = input.SortOrder;
		}
	}
}
