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
	// Missions and mysteries
	[Route("api/admin/entries")]
	public class AdminEntriesController : AdminControllerBase
	{
		public AdminEntriesController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/entries
		[HttpGet]
		public async Task<ActionResult<List<EntryAdminDto>>> GetAll()
		{
			return await (
				from e in db.Entries
				orderby e.Kind, e.StartDate, e.Title
				select new EntryAdminDto
				{
					Id = e.Id,
					Kind = e.Kind,
					Slug = e.Slug,
					Title = e.Title,
					Summary = e.Summary,
					CoverImageUrl = e.CoverImageUrl,
					StartDate = e.StartDate,
					EndDate = e.EndDate,
					Agency = e.Agency,
					MissionOutcome = e.MissionOutcome,
					EvidenceStatus = e.EvidenceStatus,
					RelatedTopicId = e.RelatedTopicId,
					IsPublished = e.IsPublished
				}).ToListAsync();
		}

		// GET /api/admin/entries/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<EntryAdminDto>> GetOne(int id)
		{
			EntryAdminDto entry = await (
				from e in db.Entries
				where e.Id == id
				select new EntryAdminDto
				{
					Id = e.Id,
					Kind = e.Kind,
					Slug = e.Slug,
					Title = e.Title,
					Summary = e.Summary,
					CoverImageUrl = e.CoverImageUrl,
					StartDate = e.StartDate,
					EndDate = e.EndDate,
					Agency = e.Agency,
					MissionOutcome = e.MissionOutcome,
					EvidenceStatus = e.EvidenceStatus,
					RelatedTopicId = e.RelatedTopicId,
					IsPublished = e.IsPublished
				}).FirstOrDefaultAsync();

			if (entry == null)
			{
				return NotFound();
			}
			return entry;
		}

		// POST /api/admin/entries
		[HttpPost]
		public async Task<ActionResult> Create(EntryInput input)
		{
			ActionResult problem = await Check(input, 0);
			if (problem != null)
			{
				return problem;
			}

			Entry entry = new Entry();
			Apply(input, entry);
			db.Entries.Add(entry);
			await db.SaveChangesAsync();
			return Created("/api/admin/entries/" + entry.Id, new { id = entry.Id });
		}

		// PUT /api/admin/entries/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, EntryInput input)
		{
			Entry entry = await db.Entries.FindAsync(id);
			if (entry == null)
			{
				return NotFound();
			}

			ActionResult problem = await Check(input, id);
			if (problem != null)
			{
				return problem;
			}

			Apply(input, entry);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/entries/5
		// Its chapters, media and people links are deleted with it. The people stay.
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			Entry entry = await db.Entries.FindAsync(id);
			if (entry == null)
			{
				return NotFound();
			}

			db.Entries.Remove(entry);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// null when everything is fine
		private async Task<ActionResult> Check(EntryInput input, int ownId)
		{
			if (input.StartDate != null && input.EndDate != null && input.EndDate < input.StartDate)
			{
				return FieldError("EndDate", "The end date can't be before the start date.");
			}
			if (input.Kind == EntryKind.Mission && input.EvidenceStatus != null)
			{
				return FieldError("EvidenceStatus", "A mission has a result, not an evidence status.");
			}
			if (input.Kind == EntryKind.Mystery && input.MissionOutcome != null)
			{
				return FieldError("MissionOutcome", "A mystery has an evidence status, not a mission result.");
			}
			if (input.RelatedTopicId != null)
			{
				bool topicExists = await (from t in db.Topics where t.Id == input.RelatedTopicId select t).AnyAsync();
				if (!topicExists)
				{
					return FieldError("RelatedTopicId", "This topic does not exist.");
				}
			}

			bool slugTaken = await (from e in db.Entries where e.Slug == input.Slug && e.Id != ownId select e).AnyAsync();
			if (slugTaken)
			{
				return ConflictMessage("Another entry already uses the slug \"" + input.Slug + "\".");
			}
			return null;
		}

		private static void Apply(EntryInput input, Entry entry)
		{
			entry.Kind = input.Kind;
			entry.Slug = input.Slug;
			entry.Title = input.Title.Trim();
			entry.Summary = input.Summary.Trim();
			entry.CoverImageUrl = TextOrEmpty(input.CoverImageUrl);
			entry.StartDate = input.StartDate;
			entry.EndDate = input.EndDate;
			entry.Agency = Optional(input.Agency);
			entry.MissionOutcome = input.MissionOutcome;
			entry.EvidenceStatus = input.EvidenceStatus;
			entry.RelatedTopicId = input.RelatedTopicId;
			entry.IsPublished = input.IsPublished;
		}
	}
}
