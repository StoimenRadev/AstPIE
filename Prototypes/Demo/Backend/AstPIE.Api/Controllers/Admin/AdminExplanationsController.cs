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
	// The cards pinned on a mystery's cork board
	[Route("api/admin/explanations")]
	public class AdminExplanationsController : AdminControllerBase
	{
		public AdminExplanationsController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/explanations or /api/admin/explanations?entryId=3
		[HttpGet]
		public async Task<ActionResult<List<ExplanationAdminDto>>> GetAll(int? entryId)
		{
			List<Explanation> rows = await (
				from x in db.Explanations
				where entryId == null || x.EntryId == entryId
				orderby x.EntryId, x.SortOrder
				select x).ToListAsync();

			List<ExplanationAdminDto> result = new List<ExplanationAdminDto>();
			foreach (Explanation row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/explanations/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<ExplanationAdminDto>> GetOne(int id)
		{
			Explanation item = await db.Explanations.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/explanations
		[HttpPost]
		public async Task<ActionResult> Create(ExplanationInput input)
		{
			if (!await ParentExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}
			if (input.ChapterId != null && !await (from c in db.Chapters where c.Id == input.ChapterId && c.EntryId == input.EntryId select c).AnyAsync())
			{
				return FieldError("ChapterId", "Choose a page of the same entry, or leave it empty for the case summary.");
			}
			Explanation item = new Explanation();
			Apply(input, item);
			db.Explanations.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/explanations/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/explanations/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, ExplanationInput input)
		{
			Explanation item = await db.Explanations.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.EntryId))
			{
				return FieldError("EntryId", "This entry does not exist.");
			}
			if (input.ChapterId != null && !await (from c in db.Chapters where c.Id == input.ChapterId && c.EntryId == input.EntryId select c).AnyAsync())
			{
				return FieldError("ChapterId", "Choose a page of the same entry, or leave it empty for the case summary.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/explanations/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Explanation item = await db.Explanations.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.Explanations.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.Entries where p.Id == id select p).AnyAsync();
		}

		private static ExplanationAdminDto ToDto(Explanation item)
		{
			ExplanationAdminDto dto = new ExplanationAdminDto();
			dto.Id = item.Id;
			dto.EntryId = item.EntryId;
			dto.ChapterId = item.ChapterId;
			dto.Title = item.Title;
			dto.ProposedBy = item.ProposedBy;
			dto.ForText = item.ForText;
			dto.AgainstText = item.AgainstText;
			dto.Verdict = item.Verdict;
			dto.Stamp = item.Stamp;
			dto.ImageUrl = item.ImageUrl;
			dto.ImageCredit = item.ImageCredit;
			dto.ImageFit = item.ImageFit;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(ExplanationInput input, Explanation item)
		{
			item.EntryId = input.EntryId;
			item.ChapterId = input.ChapterId;
			item.Title = input.Title.Trim();
			item.ProposedBy = Optional(input.ProposedBy);
			item.ForText = input.ForText.Trim();
			item.AgainstText = input.AgainstText.Trim();
			item.Verdict = input.Verdict.Trim();
			item.Stamp = input.Stamp.Trim();
			item.ImageUrl = Optional(input.ImageUrl);
			item.ImageCredit = Optional(input.ImageCredit);
			item.ImageFit = Optional(input.ImageFit);
			item.SortOrder = input.SortOrder;
		}
	}
}
