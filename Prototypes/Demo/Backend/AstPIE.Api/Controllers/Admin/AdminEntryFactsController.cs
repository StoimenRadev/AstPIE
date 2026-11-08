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
	// The rows on the cover sheet of a case
	[Route("api/admin/entry-facts")]
	public class AdminEntryFactsController : AdminControllerBase
	{
		public AdminEntryFactsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/entry-facts or /api/admin/entry-facts?entryId=3
		[HttpGet]
		public async Task<ActionResult<List<EntryFactAdminDto>>> GetAll(int? entryId)
		{
			List<EntryFact> rows = await (
				from x in db.EntryFacts
				where entryId == null || x.EntryId == entryId
				orderby x.EntryId, x.SortOrder
				select x).ToListAsync();

			List<EntryFactAdminDto> result = new List<EntryFactAdminDto>();
			foreach (EntryFact row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/entry-facts/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<EntryFactAdminDto>> GetOne(int id)
		{
			EntryFact item = await db.EntryFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/entry-facts
		[HttpPost]
		public async Task<ActionResult> Create(EntryFactInput input)
		{
			if (!await ParentExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}
			EntryFact item = new EntryFact();
			Apply(input, item);
			db.EntryFacts.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/entry-facts/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/entry-facts/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, EntryFactInput input)
		{
			EntryFact item = await db.EntryFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/entry-facts/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			EntryFact item = await db.EntryFacts.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.EntryFacts.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.Entries where p.Id == id select p).AnyAsync();
		}

		private static EntryFactAdminDto ToDto(EntryFact item)
		{
			EntryFactAdminDto dto = new EntryFactAdminDto();
			dto.Id = item.Id;
			dto.EntryId = item.EntryId;
			dto.Label = item.Label;
			dto.Value = item.Value;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(EntryFactInput input, EntryFact item)
		{
			item.EntryId = input.EntryId;
			item.Label = input.Label.Trim();
			item.Value = input.Value.Trim();
			item.SortOrder = input.SortOrder;
		}
	}
}
