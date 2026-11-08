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
	// Key words, explained in the margin of the topics that use them
	[Route("api/admin/key-words")]
	public class AdminKeyWordsController : AdminControllerBase
	{
		public AdminKeyWordsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/key-words
		[HttpGet]
		public async Task<ActionResult<List<KeyWordAdminDto>>> GetAll()
		{
			List<KeyWord> rows = await (
				from x in db.KeyWords
				orderby x.Name
				select x).ToListAsync();

			List<KeyWordAdminDto> result = new List<KeyWordAdminDto>();
			foreach (KeyWord row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/key-words/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<KeyWordAdminDto>> GetOne(int id)
		{
			KeyWord item = await db.KeyWords.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/key-words
		[HttpPost]
		public async Task<ActionResult> Create(KeyWordInput input)
		{
			if (await SlugTaken(input.Slug, 0))
			{
				return ConflictMessage("Another key word already uses this slug.");
			}
			KeyWord item = new KeyWord();
			Apply(input, item);
			db.KeyWords.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/key-words/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/key-words/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, KeyWordInput input)
		{
			KeyWord item = await db.KeyWords.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (await SlugTaken(input.Slug, id))
			{
				return ConflictMessage("Another key word already uses this slug.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/key-words/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			KeyWord item = await db.KeyWords.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.KeyWords.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// Another row (not the one with this id) that already uses the slug
		private async Task<bool> SlugTaken(string slug, int id)
		{
			return await (from x in db.KeyWords where x.Slug == slug && x.Id != id select x).AnyAsync();
		}

		private static KeyWordAdminDto ToDto(KeyWord item)
		{
			KeyWordAdminDto dto = new KeyWordAdminDto();
			dto.Id = item.Id;
			dto.Slug = item.Slug;
			dto.Name = item.Name;
			dto.OtherForms = item.OtherForms;
			dto.Definition = item.Definition;
			dto.Symbol = item.Symbol;
			dto.Unit = item.Unit;
			return dto;
		}

		private static void Apply(KeyWordInput input, KeyWord item)
		{
			item.Slug = input.Slug.Trim();
			item.Name = input.Name.Trim();
			item.OtherForms = CleanList(input.OtherForms);
			item.Definition = input.Definition.Trim();
			item.Symbol = Optional(input.Symbol);
			item.Unit = Optional(input.Unit);
		}
	}
}
