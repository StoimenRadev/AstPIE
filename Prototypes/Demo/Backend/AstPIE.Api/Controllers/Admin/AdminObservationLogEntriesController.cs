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
	// The dated entries of an object's observation log
	[Route("api/admin/log-entries")]
	public class AdminObservationLogEntriesController : AdminControllerBase
	{
		public AdminObservationLogEntriesController(AppDbContext db) : base(db)
		{
		}

		// GET /api/admin/log-entries or /api/admin/log-entries?catalogueObjectId=3
		[HttpGet]
		public async Task<ActionResult<List<LogEntryAdminDto>>> GetAll(int? catalogueObjectId)
		{
			List<ObservationLogEntry> rows = await (
				from x in db.ObservationLogEntries
				where catalogueObjectId == null || x.CatalogueObjectId == catalogueObjectId
				orderby x.CatalogueObjectId, x.SortOrder
				select x).ToListAsync();

			List<LogEntryAdminDto> result = new List<LogEntryAdminDto>();
			foreach (ObservationLogEntry row in rows)
			{
				result.Add(ToDto(row));
			}
			return result;
		}

		// GET /api/admin/log-entries/5
		[HttpGet("{id:int}")]
		public async Task<ActionResult<LogEntryAdminDto>> GetOne(int id)
		{
			ObservationLogEntry item = await db.ObservationLogEntries.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			return ToDto(item);
		}

		// POST /api/admin/log-entries
		[HttpPost]
		public async Task<ActionResult> Create(LogEntryInput input)
		{
			if (!await ParentExists(input.CatalogueObjectId))
			{
				return FieldError("CatalogueObjectId", "This catalogue object does not exist.");
			}
			ObservationLogEntry item = new ObservationLogEntry();
			Apply(input, item);
			db.ObservationLogEntries.Add(item);
			await db.SaveChangesAsync();
			return Created("/api/admin/log-entries/" + item.Id, new { id = item.Id });
		}

		// PUT /api/admin/log-entries/5
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Update(int id, LogEntryInput input)
		{
			ObservationLogEntry item = await db.ObservationLogEntries.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			if (!await ParentExists(input.CatalogueObjectId))
			{
				return FieldError("CatalogueObjectId", "This catalogue object does not exist.");
			}
			Apply(input, item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		// DELETE /api/admin/log-entries/5
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			ObservationLogEntry item = await db.ObservationLogEntries.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			db.ObservationLogEntries.Remove(item);
			await db.SaveChangesAsync();
			return NoContent();
		}

		private async Task<bool> ParentExists(int id)
		{
			return await (from p in db.CatalogueObjects where p.Id == id select p).AnyAsync();
		}

		private static LogEntryAdminDto ToDto(ObservationLogEntry item)
		{
			LogEntryAdminDto dto = new LogEntryAdminDto();
			dto.Id = item.Id;
			dto.CatalogueObjectId = item.CatalogueObjectId;
			dto.DateLabel = item.DateLabel;
			dto.Title = item.Title;
			dto.Summary = item.Summary;
			dto.Place = item.Place;
			dto.Body = item.Body;
			dto.Facts = item.Facts;
			dto.Note = item.Note;
			dto.PlateKind = item.PlateKind;
			dto.PlateUrl = item.PlateUrl;
			dto.PlateChartKey = item.PlateChartKey;
			dto.PlatePosterUrl = item.PlatePosterUrl;
			dto.PlateTitle = item.PlateTitle;
			dto.PlateCaption = item.PlateCaption;
			dto.PlateCredit = item.PlateCredit;
			dto.PlateIsDark = item.PlateIsDark;
			dto.SortOrder = item.SortOrder;
			return dto;
		}

		private static void Apply(LogEntryInput input, ObservationLogEntry item)
		{
			item.CatalogueObjectId = input.CatalogueObjectId;
			item.DateLabel = input.DateLabel.Trim();
			item.Title = input.Title.Trim();
			item.Summary = input.Summary.Trim();
			item.Place = Optional(input.Place);
			item.Body = CleanList(input.Body);
			item.Facts = CleanList(input.Facts);
			item.Note = Optional(input.Note);
			item.PlateKind = input.PlateKind;
			item.PlateUrl = Optional(input.PlateUrl);
			item.PlateChartKey = Optional(input.PlateChartKey);
			item.PlatePosterUrl = Optional(input.PlatePosterUrl);
			item.PlateTitle = Optional(input.PlateTitle);
			item.PlateCaption = Optional(input.PlateCaption);
			item.PlateCredit = Optional(input.PlateCredit);
			item.PlateIsDark = input.PlateIsDark;
			item.SortOrder = input.SortOrder;
		}
	}
}
